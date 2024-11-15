using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2;
using Amazon;
using AssemblyAI;
using AssemblyAI.Data;
using AssemblyAI.Helpers;
using AssemblyAI.Lemur;
using AssemblyAI.Transcripts;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using ThirdParty.Json.LitJson;
using Amazon.S3;

namespace AssemblyAI
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            // Set up dependency injection
            //var serviceProvider = new ServiceCollection()
            //    .AddAWSService<IAmazonDynamoDB>()
            //    .AddAWSService<IAmazonS3>()                            // Register S3 service
            //    .AddSingleton<IDynamoDBContext, DynamoDBContext>()     // Register DynamoDBContext
            //    .AddTransient<LargeDataHandler>()                      // Register LargeDataHandler for S3 upload & DynamoDB reference
            //    .BuildServiceProvider();

            Console.WriteLine("Hello, World!");
            // Make sure the `AssemblyAI` NuGet package is added to your project


            string? AssemblyAiApiKey = Environment.GetEnvironmentVariable("ASSEMBLYAI_API_KEY");
            if (string.IsNullOrEmpty(AssemblyAiApiKey))
            {
                Console.WriteLine("Error: API key not found in environment variables.");
                return;
            }
            //var fileUrl = "https://assembly.ai/sports_injuries.mp3";

            var client = new AssemblyAIClient(AssemblyAiApiKey);

            //// You can also transcribe a local file by passing in a file path
            var filePath = "C:\\Users\\m.dhissi\\source\\repos\\AssemblyAI\\AssemblyAI\\meeting.mp3";
            var uploadedFile = await client.Files.UploadAsync(new FileInfo(filePath), 
                new RequestOptions() { Timeout = TimeSpan.FromSeconds(3600)});
            var fileUrl = uploadedFile.UploadUrl;

            var transcriptParams = new TranscriptParams
            {
                AudioUrl = fileUrl,
                SpeakerLabels = true,
                AutoHighlights = true,
                AutoChapters = true,
                LanguageDetection = true,
                SentimentAnalysis = true
            };

            var transcript = await client.Transcripts.TranscribeAsync(transcriptParams);

            transcript.EnsureStatusCompleted();

            var result = new SummaryResult();

            var dictList = new List<KeyValuePair<string, int>>();
            if (transcript.Utterances is not null)
            {
                foreach (var u in transcript.Utterances)
                {
                    dictList.Add(new KeyValuePair<string, int>(u.Speaker.ToString(), u.Words.Count()));
                }
                // Group by speaker and calculate the total word count for each
                var summary = dictList
                    .GroupBy(pair => pair.Key)
                    .Select(group => new { Speaker = group.Key, TotalWords = group.Sum(pair => pair.Value) });
                foreach (var entry in summary)
                {
                    Console.WriteLine($"Speaker: {entry.Speaker}, Total Words: {entry.TotalWords}");
                    result.NumberofSpeakers = entry.Speaker.Count();
                    
                }
            }
            else
                Console.WriteLine("No Utterances available");


            result.TotalNumberOfWords = transcript.Words?.Count() ?? 0;
            result.Id = transcript.Id;
            result.AudioUrl = transcript.AudioUrl;
            result.Text = transcript.Text;
            result.DetectedLanguage = transcript.LanguageCode.ToString();
            result.AudioDuration = transcript.AudioDuration ?? 0;
            result.NumberOfChapters = transcript.Chapters?.Count() ?? 0;
            result.ConfidenceScore = transcript.Confidence ?? 0;
            result.Highlights = transcript.AutoHighlightsResult?.Results
                                .Select(x => new HighLight
                                {
                                    Text = x.Text,
                                    Count = x.Count
                                })
                                .ToList();

            result.SentimentAnalysisRecords = transcript.SentimentAnalysisResults?
                                .Select(x => new SentimentAnalysis
                                {
                                    Text = x.Text,
                                    Sentiment = x.Sentiment.ToString(),
                                    Speaker = x.Speaker
                                })
                                .ToList();
            //result.Summary = "";
            string fullFilePath = "transcription_full_result.txt";
            await WriteTranscriptToFileAsync(fullFilePath, JsonConvert.SerializeObject(result, Formatting.Indented));

            

            

            // Resolve LargeDataHandler from the service provider
            //var largeDataHandler = serviceProvider.GetRequiredService<LargeDataHandler>();

            // Upload transcript to S3 and save reference in DynamoDB
            //string bucketName = "assemblyai-challenge";  // Replace with your actual S3 bucket name
            //await largeDataHandler.StoreLargeDocumentAsync(transcript.Id.ToString(), bucketName, transcript.ToString());


            //var rootData = Root.FromJson(transcript.ToString());



            // Display
            //Console.WriteLine($"The number of words is {rootData.Words.Count()}");
            //Console.WriteLine($"The language is {rootData.LanguageCode}");
            

            // Display the summary
            

            // Persisting

            // Resolve the DynamoDBWriter instance from the service provider
            //var writer = serviceProvider.GetRequiredService<DynamoDBWriter>();
            //await writer.WriteJsonToTableAsync(transcript.Id.ToString(), transcript.ToString());


            //var response = await client.Lemur.TaskAsync(new LemurTaskParams
            //{
            //    Prompt = "Provide a brief summary of the transcript.",
            //    TranscriptIds = [transcript.Id],
            //    FinalModel = LemurModel.AnthropicClaude3_5_Sonnet
            //});

            //Console.WriteLine(response.Response);


            // Use inline statement to handle null transcript text
            //string resultText = transcript.Text ?? "No transcribed content available";
            // Write the transcription to a file
            //string textFilePath = "transcription_text.txt";
            //await WriteTranscriptToFileAsync(textFilePath, rootData.Text);

            //string fullFilePath = "transcription_full_result.txt";
            //await WriteTranscriptToFileAsync(fullFilePath, transcript.ToString());


        }

        //Method to write the transcription text to a file asynchronously
        static async Task WriteTranscriptToFileAsync(string filePath, string transcriptText)
        {
            try
            {
                // Write the transcription text to the file
                await File.WriteAllTextAsync(filePath, transcriptText);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing to file: {ex.Message}");
            }
        }

    }

}
