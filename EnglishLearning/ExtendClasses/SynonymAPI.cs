using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace EnglishLearning.ExtendClasses
{
    public class SynonymAPI
    {
        int synonymNumber = 5;

        public List<string> GetAllSynonyms(string word)
        {
            string synonyms = null;
            List<string> result = new List<string>();
            try
            {
                synonyms = ThesaurusUploadSynonyms(word);
                result = ThesaurusParse(synonyms);
            }
            catch (Exception ex)
            {
            }
            return result;
        }

        string ThesaurusUploadSynonyms(string word)
        {
            using (var client = new System.Net.WebClient())
            {
                return client.DownloadString("http://words.bighugelabs.com/api/2/775f6240f10479aba75051bf2ad1978a/" + word + "/xml");
            }
        }

        List<string> ThesaurusParse(string xml)
        {
            List<string> synonyms = new List<string>();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            XmlReader reader = new XmlNodeReader(doc);
            while (reader.ReadToFollowing("w") && synonyms.Count < synonymNumber)
            {
                reader.MoveToAttribute("r");
                string type = reader.Value;
                reader.MoveToContent();
                string result = reader.ReadInnerXml();
                if (type == "syn")
                {
                    synonyms.Add(result);
                }
            }

            return synonyms;
        }

    }
}