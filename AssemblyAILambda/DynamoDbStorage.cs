using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.Core;
using System;
using System.Threading.Tasks;

namespace AssemblyAILambda.Storage
{
    public class DynamoDbStorage
    {
        private readonly DynamoDBContext _dynamoDbContext;

        public DynamoDbStorage(IAmazonDynamoDB dynamoDbClient)
        {
            _dynamoDbContext = new DynamoDBContext(dynamoDbClient);
        }

        /// <summary>
        /// Stores the transcription result in DynamoDB.
        /// </summary>
        /// <param name="result">The transcription result to store.</param>
        /// <param name="context">The Lambda context for logging.</param>
        /// <returns></returns>
        public async Task StoreTranscriptionInDynamoDBAsync(TranscriptionResultDynamoDB result, ILambdaContext context)
        {
            try
            {
                // Save the result in DynamoDB
                await _dynamoDbContext.SaveAsync(result);
                context.Logger.LogInformation($"Transcription result stored in DynamoDB with ID: {result.Id}");
            }
            catch (Exception e)
            {
                context.Logger.LogError($"Error storing transcription in DynamoDB: {e.Message}");
                throw;
            }
        }
    }
}
