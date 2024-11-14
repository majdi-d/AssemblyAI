namespace AssemblyAI.Data
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class Root
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("audio_url")]
        public Uri AudioUrl { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("language_code")]
        public string LanguageCode { get; set; }

        [JsonProperty("language_detection")]
        public bool LanguageDetection { get; set; }

        [JsonProperty("language_confidence")]
        public double LanguageConfidence { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("words")]
        public SentimentAnalysisResult[] Words { get; set; }

        [JsonProperty("utterances")]
        public SentimentAnalysisResult[] Utterances { get; set; }

        [JsonProperty("confidence")]
        public double Confidence { get; set; }

        [JsonProperty("audio_duration")]
        public long AudioDuration { get; set; }

        [JsonProperty("punctuate")]
        public bool Punctuate { get; set; }

        [JsonProperty("format_text")]
        public bool FormatText { get; set; }

        [JsonProperty("disfluencies")]
        public bool Disfluencies { get; set; }

        [JsonProperty("dual_channel")]
        public bool DualChannel { get; set; }

        [JsonProperty("webhook_auth")]
        public bool WebhookAuth { get; set; }

        [JsonProperty("speed_boost")]
        public bool SpeedBoost { get; set; }

        [JsonProperty("auto_highlights")]
        public bool AutoHighlights { get; set; }

        [JsonProperty("auto_highlights_result")]
        public AutoHighlightsResult AutoHighlightsResult { get; set; }

        [JsonProperty("word_boost")]
        public object[] WordBoost { get; set; }

        [JsonProperty("filter_profanity")]
        public bool FilterProfanity { get; set; }

        [JsonProperty("redact_pii")]
        public bool RedactPii { get; set; }

        [JsonProperty("redact_pii_audio")]
        public bool RedactPiiAudio { get; set; }

        [JsonProperty("speaker_labels")]
        public bool SpeakerLabels { get; set; }

        [JsonProperty("content_safety")]
        public bool ContentSafety { get; set; }

        [JsonProperty("content_safety_labels")]
        public ContentSafetyLabels ContentSafetyLabels { get; set; }

        [JsonProperty("iab_categories")]
        public bool IabCategories { get; set; }

        [JsonProperty("iab_categories_result")]
        public ContentSafetyLabels IabCategoriesResult { get; set; }

        [JsonProperty("auto_chapters")]
        public bool AutoChapters { get; set; }

        [JsonProperty("chapters")]
        public Chapter[] Chapters { get; set; }

        [JsonProperty("summarization")]
        public bool Summarization { get; set; }

        [JsonProperty("custom_topics")]
        public bool CustomTopics { get; set; }

        [JsonProperty("topics")]
        public object[] Topics { get; set; }

        [JsonProperty("sentiment_analysis")]
        public bool SentimentAnalysis { get; set; }

        [JsonProperty("sentiment_analysis_results")]
        public SentimentAnalysisResult[] SentimentAnalysisResults { get; set; }

        [JsonProperty("entity_detection")]
        public bool EntityDetection { get; set; }

        [JsonProperty("throttled")]
        public bool Throttled { get; set; }

        [JsonProperty("language_model")]
        public string LanguageModel { get; set; }

        [JsonProperty("acoustic_model")]
        public string AcousticModel { get; set; }
    }

    public partial class AutoHighlightsResult
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("results")]
        public Result[] Results { get; set; }
    }

    public partial class Result
    {
        [JsonProperty("count")]
        public long Count { get; set; }

        [JsonProperty("rank")]
        public double Rank { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("timestamps")]
        public Timestamp[] Timestamps { get; set; }
    }

    public partial class Timestamp
    {
        [JsonProperty("start")]
        public long Start { get; set; }

        [JsonProperty("end")]
        public long End { get; set; }
    }

    public partial class Chapter
    {
        [JsonProperty("gist")]
        public string Gist { get; set; }

        [JsonProperty("headline")]
        public string Headline { get; set; }

        [JsonProperty("summary")]
        public string Summary { get; set; }

        [JsonProperty("start")]
        public long Start { get; set; }

        [JsonProperty("end")]
        public long End { get; set; }
    }

    public partial class ContentSafetyLabels
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("results")]
        public object[] Results { get; set; }

        [JsonProperty("summary")]
        public Summary Summary { get; set; }

        [JsonProperty("severity_score_summary", NullValueHandling = NullValueHandling.Ignore)]
        public Summary SeverityScoreSummary { get; set; }
    }

    public partial class Summary
    {
    }

    public partial class SentimentAnalysisResult
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("start")]
        public long Start { get; set; }

        [JsonProperty("end")]
        public long End { get; set; }

        [JsonProperty("sentiment", NullValueHandling = NullValueHandling.Ignore)]
        public Sentiment? Sentiment { get; set; }

        [JsonProperty("confidence")]
        public double Confidence { get; set; }

        [JsonProperty("speaker")]
        public Speaker Speaker { get; set; }

        [JsonProperty("words", NullValueHandling = NullValueHandling.Ignore)]
        public SentimentAnalysisResult[] Words { get; set; }
    }

    public enum Sentiment { Negative, Neutral, Positive };

    public enum Speaker { A, B };

    public partial class Root
    {
        public static Root FromJson(string json) => JsonConvert.DeserializeObject<Root>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this Root self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                SentimentConverter.Singleton,
                SpeakerConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class SentimentConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Sentiment) || t == typeof(Sentiment?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "NEGATIVE":
                    return Sentiment.Negative;
                case "NEUTRAL":
                    return Sentiment.Neutral;
                case "POSITIVE":
                    return Sentiment.Positive;
            }
            throw new Exception("Cannot unmarshal type Sentiment");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (Sentiment)untypedValue;
            switch (value)
            {
                case Sentiment.Negative:
                    serializer.Serialize(writer, "NEGATIVE");
                    return;
                case Sentiment.Neutral:
                    serializer.Serialize(writer, "NEUTRAL");
                    return;
                case Sentiment.Positive:
                    serializer.Serialize(writer, "POSITIVE");
                    return;
            }
            throw new Exception("Cannot marshal type Sentiment");
        }

        public static readonly SentimentConverter Singleton = new SentimentConverter();
    }

    internal class SpeakerConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Speaker) || t == typeof(Speaker?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "A":
                    return Speaker.A;
                case "B":
                    return Speaker.B;
            }
            throw new Exception("Cannot unmarshal type Speaker");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (Speaker)untypedValue;
            switch (value)
            {
                case Speaker.A:
                    serializer.Serialize(writer, "A");
                    return;
                case Speaker.B:
                    serializer.Serialize(writer, "B");
                    return;
            }
            throw new Exception("Cannot marshal type Speaker");
        }

        public static readonly SpeakerConverter Singleton = new SpeakerConverter();
    }
}