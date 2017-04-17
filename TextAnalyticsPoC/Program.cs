using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAnalyticsPoC
{
    class Program
    {
        static int DocumentLimit = 2;
        static int CallsPerMinute = 100;

        static void Main(string[] args)
        {
            ArticleManager articleMgr = new ArticleManager(@"C:\Users\zumberc\Downloads\SIGNAL_Articles_Xpress_Challenge\articles");
            //TextAnalyticsRequestManager<KeyPhrasesTextAnalyticsRequest, KeyPhrasesResponseDocument> keyPhrasesRequestMgr = new TextAnalyticsRequestManager<KeyPhrasesTextAnalyticsRequest, KeyPhrasesResponseDocument>(DocumentLimit, CallsPerMinute);
            KeyPhrasesTextAnalyticsRequestManager keyPhrasesRequestMgr = new KeyPhrasesTextAnalyticsRequestManager(DocumentLimit, CallsPerMinute);
            SentimentTextAnalyticsRequestManager sentimentRequestMgr = new SentimentTextAnalyticsRequestManager(DocumentLimit, CallsPerMinute);

            //var article = articleMgr.GetArticle("10.xml");
            //var stuff = article.Content.GetTextParts();
            //var result = requestMgr.AnalyzeArticles(new List<Article> { article });

            List<Article> articles = articleMgr.GetArticles("10.xml", "11.xml", "12.xml").ToList();
            var keyWordResult = keyPhrasesRequestMgr.AnalyzeArticles(articles);
            var sentimentResult = sentimentRequestMgr.AnalyzeArticles(articles);


            Console.ReadLine();
        }
    }
}
