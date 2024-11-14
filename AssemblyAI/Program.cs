using AssemblyAI;
using AssemblyAI.Data;
using AssemblyAI.Transcripts;
using Newtonsoft.Json;

namespace AssemblyAI
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            // Make sure the `AssemblyAI` NuGet package is added to your project


            var apiKey = "b6a1a33b3e2a49a69dc45b8b2356ee74";
            var fileUrl = "https://assembly.ai/wildfires.mp3";

            var client = new AssemblyAIClient(apiKey);

            //// You can also transcribe a local file by passing in a file path
            // var filePath = "./path/to/file.mp3";
            // var uploadedFile = await client.Files.UploadAsync(new FileInfo(filePath));
            // fileUrl = uploadedFile.UploadUrl;

            var transcriptParams = new TranscriptParams
            {
                AudioUrl = fileUrl,
                SpeakerLabels = true
            };

            var transcript = await client.Transcripts.TranscribeAsync(transcriptParams);

            transcript.EnsureStatusCompleted();
            //foreach (var x in transcript.SpeakersExpected)
            //{

            //}
            Console.WriteLine(transcript);

            //if (transcript.Text is not null)
            //{
            //    Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(transcript.Text);
            //    Console.WriteLine($"Transcription complete! {myDeserializedClass.words.Count} words in total. The text is: {myDeserializedClass.text}");
            //}

            
            // print the results
            //Console.WriteLine(myDeserializedClass.text);
        }
    }

}
