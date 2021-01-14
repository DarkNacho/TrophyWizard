using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using TrophyParser.Models;

namespace TrophyParser
{
    public abstract class SFMParse
    {
        public abstract SFMHearder Header { get;}
        public abstract Trophy this[int index] { get; }
        public abstract bool HasPlatinum { get; set; }
        public abstract int Count { get; }
        protected virtual IEnumerable<Trophy> Parse(FileStream reader)
        {
            //reader.Position = start;
            var data = new byte[reader.Length];
            reader.Read(data, 0, (int) reader.Length);
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(Encoding.UTF8.GetString(data).Trim('\0'));
            var root = xmlDoc.DocumentElement;
            // Fixed Data
            Header.trophyconf_version = root.Attributes["version"].Value;
            Header.npcommid = xmlDoc.GetElementsByTagName("npcommid")[0].InnerText;
            Header.trophyset_version = xmlDoc.GetElementsByTagName("trophyset-version")[0].InnerText;
            Header.parental_level = xmlDoc.GetElementsByTagName("parental-level")[0].InnerText;
            Header.title_name = xmlDoc.GetElementsByTagName("title-name")[0].InnerText;
            Header.title_detail = xmlDoc.GetElementsByTagName("title-detail")[0].InnerText;

            var trophiesXml = xmlDoc.GetElementsByTagName("trophy");
            var trophies = new List<Trophy>();
            foreach (XmlNode trophy in trophiesXml)
            {
                Trophy item = new Trophy
                (
                    int.Parse(trophy.Attributes["id"].Value),
                    trophy.Attributes["hidden"].Value,
                    trophy.Attributes["ttype"].Value,
                    int.Parse(trophy.Attributes["pid"].Value),
                    trophy["name"].InnerText,
                    trophy["detail"].InnerText,
                    int.Parse(trophy.Attributes["gid"]?.Value ?? "0")
                );
                trophies.Add(item);
            }
            HasPlatinum = trophies[0].Type.Equals("P");
            reader.Close();
            return trophies;
        }
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(Header.trophyconf_version);
            sb.AppendLine(Header.npcommid);
            sb.AppendLine(Header.trophyset_version);
            sb.AppendLine(Header.parental_level);
            sb.AppendLine(Header.title_name);
            sb.AppendLine(Header.title_detail);
            sb.AppendLine();
            return sb.ToString();
        }
    }
}