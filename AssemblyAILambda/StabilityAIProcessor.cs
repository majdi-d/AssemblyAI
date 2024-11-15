using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AssemblyAILambda;
using AssemblyAILambda.Data;
using AssemblyAI;
using AssemblyAI.Transcripts;

namespace AssemblyAILambda.Processor
{
    public static class StabilityAIProcessor
    {
        public static async Task<SummaryResult> ProcessTranscriptionAsync(AssemblyAIClient client, string fileUrl)
        {
            // Upload the file and get the file URL
            //var uploadedFile = await client.Files.UploadAsync(new FileInfo(fileUrl),
            //    new RequestOptions() { Timeout = TimeSpan.FromSeconds(3600) });
            //var fileUrl = uploadedFile.UploadUrl;

            // Set up the transcription parameters
            var transcriptParams = new TranscriptParams
            {
                AudioUrl = fileUrl,
                SpeakerLabels = true,
                AutoHighlights = true,
                AutoChapters = true,
                LanguageDetection = true,
                SentimentAnalysis = true
            };

            // Request transcription
            var transcript = await client.Transcripts.TranscribeAsync(transcriptParams, 
                new RequestOptions() { Timeout = TimeSpan.FromSeconds(3600)});
            transcript.EnsureStatusCompleted();

            // Process the results into a summary
            var result = new SummaryResult();
            var dictList = new List<KeyValuePair<string, int>>();

            // If there are utterances, process them
            if (transcript.Utterances != null)
            {
                foreach (var u in transcript.Utterances)
                {
                    dictList.Add(new KeyValuePair<string, int>(u.Speaker.ToString(), u.Words.Count()));
                }

                // Group by speaker and calculate total word count
                var summary = dictList
                    .GroupBy(pair => pair.Key)
                    .Select(group => new { Speaker = group.Key, TotalWords = group.Sum(pair => pair.Value) });

                foreach (var entry in summary)
                {
                    //Console.WriteLine($"Speaker: {entry.Speaker}, Total Words: {entry.TotalWords}");
                    result.NumberofSpeakers = entry.Speaker.Count();
                }
            }
            else
            {
                Console.WriteLine("No Utterances available in the transcribed audio");
            }

            // Fill in the result with additional transcription data
            result.TotalNumberOfWords = transcript.Words?.Count() ?? 0;
            result.Id = transcript.Id;
            result.AudioUrl = transcript.AudioUrl;
            result.Text = transcript.Text;
            result.DetectedLanguage = transcript.LanguageCode.ToString();
            result.AudioDuration = transcript.AudioDuration ?? 0;
            result.NumberOfChapters = transcript.Chapters?.Count() ?? 0;
            result.ConfidenceScore = transcript.Confidence ?? 0;

            // Process highlights
            result.Highlights = transcript.AutoHighlightsResult?.Results
                .Select(x => new HighLight
                {
                    Text = x.Text,
                    Count = x.Count
                })
                .ToList();

            // Process sentiment analysis records
            result.SentimentAnalysisRecords = transcript.SentimentAnalysisResults?
                .Select(x => new SentimentAnalysis
                {
                    Text = x.Text,
                    Sentiment = x.Sentiment.ToString(),
                    Speaker = x.Speaker
                })
                .ToList();

            return result;
        }
    }
}
