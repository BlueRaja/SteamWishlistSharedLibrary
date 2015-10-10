using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using CsQuery;

namespace SteamWishlist
{
    public class SteamOwnedGamesRetriever
    {
        public async Task<IEnumerable<SteamGame>> GetOwnedGames(string profileUrl)
        {
            Uri gamesUrl = new Uri(profileUrl).Append("games/?xml=1");
            using(WebClient client = new WebClient())
            {
                string gamesXml = await client.DownloadStringTaskAsync(gamesUrl);
                XDocument root = XDocument.Parse(gamesXml);
                return ParseGamesXml(root);
            }
        }

        private IEnumerable<SteamGame> ParseGamesXml(XDocument root)
        {
            foreach (XElement gameNode in root.Descendants("game"))
            {
                string name = RemoveCData(gameNode.Element("name").Value);
                string url = RemoveCData(gameNode.Element("storeLink").Value);
                yield return new SteamGame(name, url);
            }
        }

        private string RemoveCData(string str)
        {
            if (str.StartsWith("<![CDATA["))
            {
                str = str.Substring(8, str.Length - 11);
            }
            return str;
        }
    }
}
