using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TextAnalyticsPoC
{
    public class Article
    {
        public string Filename { get; set; }
        //public int Id { get; set; }
        public string Id { get; set; }

        public ArticleContent Content { get; set; }
    }

    [XmlRoot(ElementName = "article")]
    public class ArticleContent
    {
        public string type { get; set; }
        public string title { get; set; }
        public string author { get; set; }
        public string date { get; set; }
        public List<ArticleDepartment> departments { get; set; }
        public List<ArticleTag> tags { get; set; }

        [XmlElement(ElementName = "abstract")]
        public string @abstract { get; set; }
        public string text { get; set; }
        public string imgalttext { get; set; }

        public List<string> GetTextParts()
        {
            // TODO break on periods?
            List<string> parts = new List<string>();

            byte[] textBytes = System.Text.Encoding.Unicode.GetBytes(text);
            int numParts = (int)Math.Round((textBytes.Length / 10240.0), MidpointRounding.AwayFromZero);

            for (int i = 0; i < numParts; i++)
            {
                int partByteLength = textBytes.Length - (10240 * i) >= 10240 ? 10240 : textBytes.Length - (10240 * i);

                byte[] partBytes = new byte[partByteLength];
                Array.Copy(textBytes, i * 10240, partBytes, 0, partByteLength);

                char[] chars = new char[Encoding.Unicode.GetCharCount(partBytes, 0, partBytes.Length)];
                Encoding.Unicode.GetChars(partBytes, 0, partBytes.Length, chars, 0);

                string textPart = new string(chars);
                parts.Add(textPart);
            }

            return parts;
        }
    }

    public class ArticleDepartment
    {
        public string department { get; set; }
    }

    public class ArticleTag
    {
        public string tag { get; set; }
    }
}
