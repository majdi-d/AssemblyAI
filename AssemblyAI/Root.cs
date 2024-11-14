using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssemblyAI.Data
{
    public class ContentSafetyLabels
    {
        public string status { get; set; }
        public List<object> results { get; set; }
        public Summary summary { get; set; }
        public SeverityScoreSummary severity_score_summary { get; set; }
    }

    public class IabCategoriesResult
    {
        public string status { get; set; }
        public List<object> results { get; set; }
        public Summary summary { get; set; }
    }

    public class Root
    {
        public string id { get; set; }
        public string audio_url { get; set; }
        public string status { get; set; }
        public string language_code { get; set; }
        public bool language_detection { get; set; }
        public string text { get; set; }
        public List<Word> words { get; set; }
        public double confidence { get; set; }
        public int audio_duration { get; set; }
        public bool punctuate { get; set; }
        public bool format_text { get; set; }
        public bool disfluencies { get; set; }
        public bool webhook_auth { get; set; }
        public bool speed_boost { get; set; }
        public bool auto_highlights { get; set; }
        public List<object> word_boost { get; set; }
        public bool filter_profanity { get; set; }
        public bool redact_pii { get; set; }
        public bool redact_pii_audio { get; set; }
        public bool speaker_labels { get; set; }
        public bool content_safety { get; set; }
        public ContentSafetyLabels content_safety_labels { get; set; }
        public bool iab_categories { get; set; }
        public IabCategoriesResult iab_categories_result { get; set; }
        public bool auto_chapters { get; set; }
        public bool summarization { get; set; }
        public bool custom_topics { get; set; }
        public List<object> topics { get; set; }
        public bool sentiment_analysis { get; set; }
        public bool entity_detection { get; set; }
        public bool throttled { get; set; }
        public string language_model { get; set; }
        public string acoustic_model { get; set; }
    }

    public class SeverityScoreSummary
    {
    }

    public class Summary
    {
    }

    public class Word
    {
        public double confidence { get; set; }
        public int start { get; set; }
        public int end { get; set; }
        public string text { get; set; }
    }

}
