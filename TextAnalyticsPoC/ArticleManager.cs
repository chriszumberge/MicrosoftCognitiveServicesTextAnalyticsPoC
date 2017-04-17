using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TextAnalyticsPoC
{
    public sealed class ArticleManager
    {
        string mFolderPath { get; set; }

        List<FileInfo> mArticleFiles { get; set; } = new List<FileInfo>();

        public ArticleManager(string pathToArticles)
        {
            if (!Directory.Exists(pathToArticles))
            {
                throw new ArgumentException($"Directory '{pathToArticles}' does not exist.");
            }

            DirectoryInfo directory = new DirectoryInfo(pathToArticles);
            mArticleFiles = directory.GetFiles("*.*").ToList();
        }

        public IEnumerable<string> GetArticleNames(int skip, int take)
        {
            return mArticleFiles.Skip(skip).Take(take).Select(x => x.Name);
        }

        public IEnumerable<Article> GetArticles(int skip, int take)
        {
            IEnumerable<string> fileNames = GetArticleNames(skip, take);
            return fileNames.Select(x => GetArticle(x));
        }

        public IEnumerable<Article> GetArticles(params string[] articleFileNames)
        {
            return articleFileNames.Select(x => GetArticle(x));
        }

        public Article GetArticle(string articleFileName)
        {
            Article article = null;

            FileInfo fileInfo = mArticleFiles.FirstOrDefault(x => x.Name.Equals(articleFileName));

            if (fileInfo != null)
            {
                article = new Article();
                article.Filename = fileInfo.Name;
                //article.Id = Int32.Parse(fileInfo.Name.Substring(0, fileInfo.Name.LastIndexOf(".")));
                article.Id = fileInfo.Name.Substring(0, fileInfo.Name.LastIndexOf("."));

                XmlSerializer serializer = new XmlSerializer(typeof(ArticleContent));
                FileStream fileStream = new FileStream(fileInfo.FullName, FileMode.Open);
                ArticleContent content = (ArticleContent)serializer.Deserialize(fileStream);
                article.Content = content;
            }

            return article;
        }
    }
}
