using Amazon.S3;
using Amazon.S3.Model;
using System.Text.Json;
using System.Threading.Tasks;

namespace AssemblyAILambda.Storage
{
    public static class TranscriptStorage
    {
        /// <summary>
        /// Uploads a transcription result to an S3 bucket as a JSON file.
        /// </summary>
        /// <param name="s3Client">The Amazon S3 client.</param>
        /// <param name="bucketName">The target S3 bucket name.</param>
        /// <param name="objectKey">The S3 object key (file name).</param>
        /// <param name="transcriptionResult">The transcription result to store.</param>
        /// <returns></returns>
        public static async Task UploadTranscriptionToS3Async(
            IAmazonS3 s3Client,
            string bucketName,
            string objectKey,
            object transcriptionResult)
        {
            // Serialize the transcription result to JSON
            var jsonData = JsonSerializer.Serialize(transcriptionResult, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            // Create a PutObjectRequest
            var putRequest = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = objectKey,
                ContentBody = jsonData,
                ContentType = "application/json"
            };

            // Upload the JSON file to S3
            await s3Client.PutObjectAsync(putRequest);
        }
    }
}
