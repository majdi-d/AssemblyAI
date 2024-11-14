using Amazon.S3;
using Amazon.S3.Model;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using System;
using System.Threading.Tasks;
using AssemblyAI.Data;

namespace AssemblyAI.Helpers
{
    public class LargeDataHandler
    {
        private readonly IAmazonS3 _s3Client;
        private readonly IDynamoDBContext _dynamoDbContext;

        public LargeDataHandler(IAmazonS3 s3Client, IDynamoDBContext dynamoDbContext)
        {
            _s3Client = s3Client;
            _dynamoDbContext = dynamoDbContext;
        }

        public async Task StoreLargeDocumentAsync(string documentId, string bucketName, string jsonData)
        {
            // Step 1: Upload JSON data to S3
            var putRequest = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = documentId,
                ContentBody = jsonData,
                ContentType = "application/json" // Set the content type
            };

            await _s3Client.PutObjectAsync(putRequest);

            // Step 2: Generate S3 URL for reference (optional but useful)
            string s3Url = $"https://{bucketName}.s3.amazonaws.com/{documentId}.json";

            // Step 3: Store reference to S3 object in DynamoDB
            var item = new AssemblyAIDynamoDBModel
            {
                Id = documentId,
                S3Key = documentId, // S3 Key reference
                S3Url = s3Url       // Optional: Full S3 URL for easy retrieval
            };

            await _dynamoDbContext.SaveAsync(item);
        }
    }
}