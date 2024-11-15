namespace AssemblyAI.Data
{
    public class SummaryResult
    {
        public string? Id { get; set; }
        public string?  AudioUrl { get; set; }
        public string?  DetectedLanguage { get; set; }
        public string?  Text { get; set; }
        public int? TotalNumberOfWords { get; set; }
        public int  NumberofSpeakers { get; set; }
        public string? Summary {  get; set; }   
        public int NumberOfChapters { get; set; }
        public double? ConfidenceScore { get; set; }
        public int AudioDuration { get; set; }
        public List<HighLight>? Highlights { get; set; } = new ();
        public List<SentimentAnalysis>? SentimentAnalysisRecords { get; set; } = new();
    }

    public class HighLight
    {
        public int Count { get; set; } = 0;
        public string? Text { get; set; } = string.Empty;
    }

    public class SentimentAnalysis
    {
        public string? Text { get; set; }
        public string? Sentiment { get; set; }
        public string? Speaker { get; set; }
    }
}
