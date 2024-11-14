using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;

namespace AssemblyAI.Data
{
    [DynamoDBTable("AssemblyAI")]
    public class AssemblyAIDynamoDBModel
    {
        [DynamoDBHashKey]
        public string? Id { get; set; }

        [DynamoDBProperty]
        public string? S3Key { get; set; }

        [DynamoDBProperty]
        public string? S3Url { get; set; } // Optional: Full URL for direct access
    }
}
