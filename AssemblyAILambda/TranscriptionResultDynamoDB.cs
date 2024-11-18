using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;

[DynamoDBTable("AssemblyAI")] // DynamoDB table name
public class TranscriptionResultDynamoDB
{
    [DynamoDBHashKey] // Partition key
    public string? Id { get; set; }

    [DynamoDBProperty] // This decorates a normal attribute
    public string? Text { get; set; }

    [DynamoDBProperty] // This decorates a normal attribute
    public string? Timestamp { get; set; }
}
