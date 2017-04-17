using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAnalyticsPoC
{
    [Serializable]
    public class AzureDocumentsList<T> where T : Document
    {
        public List<T> documents { get; set; } = new List<T>();
        public List<Error> errors { get; set; } = new List<Error>();
    }

    [Serializable]
    public class Error
    {
        //public int id { get; set; }
        public string id { get; set; }
        public string message { get; set; }
    }

    [Serializable]
    public class TopicsDocumentsList<T> : Document where T : Document
    {
        /// <summary>
        /// Gets or sets the stop words.
        /// </summary>
        /// <remarks>
        /// These words and their close forms (e.g. plurals) will be excluded from the entire topic detection pipeline. 
        /// Use this for common words (for example, “issue”, “error” and “user” may be appropriate choices for customer complaints about software). 
        /// Each string should be a single word.
        /// </remarks>
        /// <value>
        /// The stop words.
        /// </value>
        public List<string> stopWords { get; set; } = new List<string>();
        /// <summary>
        /// Gets or sets the stop phrases.
        /// </summary>
        /// <remarks>
        /// These phrases will be excluded from the list of returned topics. 
        /// Use this to exclude generic topics that you don’t want to see in the results. 
        /// For example, “Microsoft” and “Azure” would be appropriate choices for topics to exclude. 
        /// Strings can contain multiple words.
        /// </remarks>
        /// <value>
        /// The stop phrases.
        /// </value>
        public List<string> stopPhrases { get; set; } = new List<string>();
    }

    [Serializable]
    public class Document
    {
        //public int id { get; set; }
        public string id { get; set; }
    }

    [Serializable]
    public class RequestDocument : Document
    {
        public string language = "en";
        public string text { get; set; }
    }

    [Serializable]
    public abstract class ResponseDocument : Document
    {
        public abstract void AddResponseData(ResponseDocument doc);
    }

    [Serializable]
    public class SentimentResponseDocument : ResponseDocument
    {
        public double score { get; set; }

        private double docCount { get; set; } = 0;
        public override void AddResponseData(ResponseDocument doc)
        {
            SentimentResponseDocument sentimentDoc = doc as SentimentResponseDocument;

            docCount++;

            score = (score + sentimentDoc.score) / docCount;
        }
    }

    [Serializable]
    public class KeyPhrasesResponseDocument : ResponseDocument
    {
        public List<string> keyPhrases { get; set; } = new List<string>();

        public override void AddResponseData(ResponseDocument doc)
        {
            KeyPhrasesResponseDocument keyPhrasesDoc = doc as KeyPhrasesResponseDocument;
            keyPhrases.AddRange(keyPhrasesDoc.keyPhrases);

            keyPhrases = keyPhrases.Distinct().ToList();
        }
    }

    [Serializable]
    public class LanguagesResponseDocument : ResponseDocument
    {
        public List<Language> detectedLanguages { get; set; } = new List<Language>();

        public override void AddResponseData(ResponseDocument doc)
        {
            LanguagesResponseDocument languagesDoc = doc as LanguagesResponseDocument;
            detectedLanguages.AddRange(languagesDoc.detectedLanguages);

            detectedLanguages = detectedLanguages.Distinct().ToList();
        }
    }

    [Serializable]
    public class Language
    {
        public string name { get; set; }
        public string iso6391Name { get; set; }
        public double score { get; set; }
    }
}
