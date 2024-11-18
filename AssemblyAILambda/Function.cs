using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using AssemblyAI;
using AssemblyAILambda.Processor;
using AssemblyAILambda.Storage;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace AssemblyAILambda;

public class Function
{
    IAmazonS3 S3Client { get; set; }
    IAmazonDynamoDB DynamoDbClient { get; set; }
    /// <summary>
    /// Default constructor. This constructor is used by Lambda to construct the instance. When invoked in a Lambda environment
    /// the AWS credentials will come from the IAM role associated with the function and the AWS region will be set to the
    /// region the Lambda function is executed in.
    /// </summary>
    public Function()
    {
        S3Client = new AmazonS3Client();
        DynamoDbClient = new AmazonDynamoDBClient();
    }

    /// <summary>
    /// Constructs an instance with a preconfigured S3 client. This can be used for testing outside of the Lambda environment.
    /// </summary>
    /// <param name="s3Client"></param>
    public Function(IAmazonS3 s3Client, IAmazonDynamoDB dynamoDbClient)
    {
        this.S3Client = s3Client;
        this.DynamoDbClient = dynamoDbClient;
    }

    /// <summary>
    /// This method is called for every Lambda invocation. This method takes in an S3 event object and can be used 
    /// to respond to S3 notifications.
    /// </summary>
    /// <param name="evnt"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task FunctionHandler(S3Event evnt, ILambdaContext context)
    {
        // Read environment variable (e.g., API key or other configuration values)
        string? apiKey = Environment.GetEnvironmentVariable("ASSEMBLYAI_API_KEY");
        if (string.IsNullOrEmpty(apiKey))
        {
            context.Logger.LogError("Error: API key not found in environment variables.");
            return;
        }
        else
        {
            context.Logger.LogInformation($"API Key is successfully loaded, ending with ****{apiKey.Substring(apiKey.Length - 4)}");
        }


        var eventRecords = evnt.Records ?? new List<S3Event.S3EventNotificationRecord>();
        foreach (var record in eventRecords)
        {
            var s3Event = record.S3;
            if (s3Event == null)
            {
                continue;
            }

            try
            {
                var response = await this.S3Client.GetObjectMetadataAsync(s3Event.Bucket.Name, s3Event.Object.Key);
                context.Logger.LogInformation(response.Headers.ContentType);
                // Construct the full S3 file path
                var bucketName = s3Event.Bucket.Name;
                var objectKey = s3Event.Object.Key;
                // Download the file to /tmp (Lambda's temporary storage)
                //var tmpFilePath = Path.Combine("/tmp", objectKey);
                //await DownloadFileFromS3(bucketName, objectKey, tmpFilePath);




                var fullFilePath = $"s3://{bucketName}/{objectKey}";
                context.Logger.LogInformation($"The S3 file path is : {fullFilePath}");
                var client = new AssemblyAIClient(Environment.GetEnvironmentVariable("ASSEMBLYAI_API_KEY"));
                // Generate a pre-signed URL for StabilityAI to be able to access the file
                var fileUrl = GeneratePresignedUrl(bucketName, objectKey);
                context.Logger.LogInformation($"The pre-signed S3 file path is : {fileUrl}");
                // Process the transcription using the StabilityAIProcessor
                context.Logger.LogInformation($"Starting to process the transcription using StabilityAI API");
                var result = await StabilityAIProcessor.ProcessTranscriptionAsync(client, fileUrl);
                // Log or process the result
                context.Logger.LogInformation($"Processed transcription with ID: {result.Id}");
                // You can also store the result in S3, DynamoDB, etc.
                context.Logger.LogInformation($"The transcribed/un-escaped text is: {Regex.Unescape(result.Text ?? "No text available")}");

                // Store transcription result in DynamoDB
                var item = new Amazon.DynamoDBv2.DocumentModel.Document
                {
                    ["Id"] = result.Id,
                    ["Text"] = result.Text ?? "No text available",
                    ["Timestamp"] = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
                };

                await StoreTranscriptionInDynamoDB(item, context);

                context.Logger.LogInformation($"Transcription result stored in DynamoDB with ID: {result.Id}");


                // Store the transcription result in the S3 bucket
                var outputObjectKey = $"{objectKey}-transcription.json";
                var targetBucket = "assemblyai-challenge-transcripts";
                await TranscriptStorage.UploadTranscriptionToS3Async(S3Client, targetBucket, outputObjectKey, result);
                context.Logger.LogInformation($"Transcription result uploaded to S3 {targetBucket} bucket under {outputObjectKey}");
            }
            catch (Exception e)
            {
                context.Logger.LogError($"Error getting object {s3Event.Object.Key} from bucket {s3Event.Bucket.Name}. Make sure they exist and your bucket is in the same region as this function.");
                context.Logger.LogError(e.Message);
                context.Logger.LogError(e.StackTrace);
                throw;
            }
        }

    }
    public string GeneratePresignedUrl(string bucketName, string objectKey)
    {
        var s3Client = new AmazonS3Client();
        var request = new GetPreSignedUrlRequest
        {
            BucketName = bucketName,
            Key = objectKey,
            Expires = DateTime.UtcNow.AddSeconds(120) // URL expiration time
        };
        return s3Client.GetPreSignedURL(request);
    }

    private async Task StoreTranscriptionInDynamoDB(Amazon.DynamoDBv2.DocumentModel.Document transcriptionItem, ILambdaContext context)
    {
        var putItemRequest = new PutItemRequest
        {
            TableName = "AssemblyAI",
            Item = transcriptionItem.ToAttributeMap()
        };

        try
        {
            await DynamoDbClient.PutItemAsync(putItemRequest);
            context.Logger.LogInformation("Transcription data successfully stored in DynamoDB.");
        }
        catch (Exception ex)
        {
            context.Logger.LogError("Error storing transcription data in DynamoDB.");
            context.Logger.LogError(ex.Message);
            throw;
        }
    }

    //private async Task DownloadFileFromS3(string bucketName, string objectKey, string tmpFilePath)
    //{
    //    using (var s3Client = new AmazonS3Client())
    //    {
    //        var request = new GetObjectRequest
    //        {
    //            BucketName = bucketName,
    //            Key = objectKey
    //        };

    //        using (var response = await s3Client.GetObjectAsync(request))
    //        {
    //            await response.WriteResponseStreamToFileAsync(tmpFilePath);
    //        }
    //    }
    //}
}