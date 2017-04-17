using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TextAnalyticsPoC
{
    public class TextAnalyticsRequestManager<T, U> where T : TextAnalyticsRequest
                                                          where U : ResponseDocument, new()
    {
        TextAnalyticsRequest mRequester { get; set; }

        readonly int mDocumentLimit;
        readonly int mCallsPerMinute;
        int mCallsPerSecond => mCallsPerMinute / 60;
        public TextAnalyticsRequestManager(int documentLimit, int callsPerMinute)
        {
            mRequester = Activator.CreateInstance<T>();
            mDocumentLimit = documentLimit;
            mCallsPerMinute = callsPerMinute;
        }

        DateTime mLastRequestTime { get; set; }
        public AzureDocumentsList<U> AnalyzeArticles(List<Article> articles)
        {
            AzureDocumentsList<U> result = new AzureDocumentsList<U>();
            Dictionary<string, U> articlePartDocumentDictionary = new Dictionary<string, U>();
            Dictionary<string, Error> articlePartErrorDictionary = new Dictionary<string, Error>();

            // TODO break apart the text parts as seperate documents since they all add to the document limit
            List<Article> splitArticles = new List<Article>();
            foreach (Article article in articles)
            {
                articlePartDocumentDictionary.Add(article.Id, new U()
                {
                    id = article.Id
                });
                articlePartErrorDictionary.Add(article.Id, new Error()
                {
                    id = article.Id
                });

                List<string> articleTextParts = article.Content.GetTextParts();
                int textPartCount = 1;
                foreach (string articleTextPart in articleTextParts)
                {
                    splitArticles.Add(
                        new Article()
                        {
                            Id = String.Concat(article.Id, "-", textPartCount),
                            //Id = article.Id + textPartCount,
                            Content = new ArticleContent
                            {
                                text = articleTextPart
                            }
                        }
                    );
                    textPartCount++;
                }
            }

            for (int i = 0; i < splitArticles.Count; i += mDocumentLimit)
            {
                IEnumerable<Article> postable = splitArticles.Skip(i).Take(mDocumentLimit);

                List<RequestDocument> documents = postable.Select(x => new RequestDocument() { id = x.Id, text = x.Content.text }).ToList();

                if (mLastRequestTime != null)
                {
                    double secondsSinceLastCall = (mLastRequestTime - DateTime.Now).TotalSeconds;

                    if (secondsSinceLastCall > 0 && (secondsSinceLastCall < mCallsPerSecond))
                    {
                        Thread.Sleep((int)Math.Round(mCallsPerSecond - secondsSinceLastCall, 0));
                    }
                }

                mLastRequestTime = DateTime.Now;
                var analysisResults = mRequester.AnalyzeDocuments<U>(new AzureDocumentsList<RequestDocument>
                {
                    documents = documents
                });

                // Combine broken parts of articles to a single result document/error for each article
                foreach (var analysisResultDoc in analysisResults.documents)
                {
                    string id = analysisResultDoc.id.Substring(0, analysisResultDoc.id.LastIndexOf("-"));
                    var doc = articlePartDocumentDictionary[id];
                    if (doc != null)
                    {
                        doc.AddResponseData(analysisResultDoc);
                    }
                }

                foreach (var analysisResultError in analysisResults.errors)
                {
                    string id = analysisResultError.id.Substring(0, analysisResultError.id.LastIndexOf("-"));
                    var error = articlePartErrorDictionary[id];
                    if (error != null)
                    {
                        error.message = String.Concat(error.message, Environment.NewLine, analysisResultError.message);
                    }
                }

                //documentParts.AddRange(analysisResult.documents);
                //errorParts.AddRange(analysisResult.errors);
            }

            result.documents.AddRange(articlePartDocumentDictionary.Select(x => x.Value).ToList());
            result.errors.AddRange(articlePartErrorDictionary.Where(x => !String.IsNullOrEmpty(x.Value.message)).Select(x => x.Value).ToList());


            //result.documents.AddRange(analysisResult.documents);
            //result.errors.AddRange(analysisResult.errors);

            return result;
        }
    }

    public class SentimentTextAnalyticsRequestManager : TextAnalyticsRequestManager<SentimentTextAnalyticsRequest, SentimentResponseDocument>
    {
        public SentimentTextAnalyticsRequestManager(int documentLimit, int callsPerMinute) : base(documentLimit, callsPerMinute)
        {
        }
    }

    public class KeyPhrasesTextAnalyticsRequestManager : TextAnalyticsRequestManager<KeyPhrasesTextAnalyticsRequest, KeyPhrasesResponseDocument>
    {
        public KeyPhrasesTextAnalyticsRequestManager(int documentLimit, int callsPerMinute) : base(documentLimit, callsPerMinute)
        {
        }
    }

    public class LanguagesTextAnalyticsRequestManager : TextAnalyticsRequestManager<LanguagesTextAnalyticsRequest, LanguagesResponseDocument>
    {
        public LanguagesTextAnalyticsRequestManager(int documentLimit, int callsPerMinute) : base(documentLimit, callsPerMinute)
        {
        }
    }
}
