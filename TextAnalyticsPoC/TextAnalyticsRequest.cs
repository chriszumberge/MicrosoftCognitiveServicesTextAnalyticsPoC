using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TextAnalyticsPoC
{
    public abstract class TextAnalyticsRequest
    {
        const string ApiUrlBase = "https://westus.api.cognitive.microsoft.com/text/analytics/v2.0/";

        readonly Uri requestUri;

        private static readonly HttpClient client = new HttpClient();

        public TextAnalyticsRequest(string endpoint)
        {
            requestUri = new Uri(String.Concat(ApiUrlBase, endpoint));
        }

        public abstract AzureDocumentsList<T> AnalyzeDocuments<T>(AzureDocumentsList<RequestDocument> documents) where T : ResponseDocument;

        protected async Task<HttpResponseMessage> CallApi(AzureDocumentsList<RequestDocument> documents)
        {
            //var httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUri);
            //httpWebRequest.ContentType = "application/json";
            //httpWebRequest.Method = "POST";
            //httpWebRequest.Accept = "application/json";
            //httpWebRequest.Headers.Add("Ocp-Apim-Subscription-Key", "92a9360b7d654314bffeba9ef388de67");

            var request = new HttpRequestMessage()
            {
                RequestUri = requestUri,
                Method = HttpMethod.Post
            };
            request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            //request.Headers.Add("ContentType", "application/json");
            request.Headers.AcceptEncoding.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("UTF-8"));
            request.Headers.Add("Ocp-Apim-Subscription-Key", "92a9360b7d654314bffeba9ef388de67");
            request.Content = new StringContent(JsonConvert.SerializeObject(documents), Encoding.UTF8, "application/json");

            HttpResponseMessage responseMessage = await client.SendAsync(request);

            return responseMessage;
        }
    }

    public sealed class SentimentTextAnalyticsRequest : TextAnalyticsRequest
    {
        public SentimentTextAnalyticsRequest() : base("sentiment")
        {

        }

        public override AzureDocumentsList<SentimentResponseDocument> AnalyzeDocuments<SentimentResponseDocument>(AzureDocumentsList<RequestDocument> documents)
        {
            HttpResponseMessage response = this.CallApi(documents).Result;
            string responseContentString = response.Content.ReadAsStringAsync().Result;
            AzureDocumentsList<SentimentResponseDocument> responseContent = JsonConvert.DeserializeObject<AzureDocumentsList<SentimentResponseDocument>>(responseContentString);
            return responseContent;
        }
    }

    public sealed class KeyPhrasesTextAnalyticsRequest : TextAnalyticsRequest
    {
        public KeyPhrasesTextAnalyticsRequest() : base("keyPhrases")
        {

        }

        public override AzureDocumentsList<KeyPhrasesResponseDocument> AnalyzeDocuments<KeyPhrasesResponseDocument>(AzureDocumentsList<RequestDocument> documents)
        {
            HttpResponseMessage response = this.CallApi(documents).Result;
            string responseContentString = response.Content.ReadAsStringAsync().Result;
            AzureDocumentsList<KeyPhrasesResponseDocument> responseContent = JsonConvert.DeserializeObject<AzureDocumentsList<KeyPhrasesResponseDocument>>(responseContentString);
            return responseContent;
        }
    }

    public sealed class LanguagesTextAnalyticsRequest : TextAnalyticsRequest
    {
        public LanguagesTextAnalyticsRequest() : base("languages")
        {

        }

        public override AzureDocumentsList<LanguagesResponseDocument> AnalyzeDocuments<LanguagesResponseDocument>(AzureDocumentsList<RequestDocument> documents)
        {
            HttpResponseMessage response = this.CallApi(documents).Result;
            string responseContentString = response.Content.ReadAsStringAsync().Result;
            AzureDocumentsList<LanguagesResponseDocument> responseContent = JsonConvert.DeserializeObject<AzureDocumentsList<LanguagesResponseDocument>>(responseContentString);
            return responseContent;
        }
    }

    public sealed class TopicsTextAnalyticsRequest : TextAnalyticsRequest
    {

        public TopicsTextAnalyticsRequest() : base("topics")
        {

        }

        public override AzureDocumentsList<T> AnalyzeDocuments<T>(AzureDocumentsList<RequestDocument> documents)
        {
            throw new NotImplementedException();
        }
    }
}
