﻿using System;
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
        public async Task<SteamGamesList> GetOwnedGames(string profileUrl)
        {
            WebClient webClient = new WebClient { Proxy = null }; //See http://stackoverflow.com/questions/4415443 - ugh.
            Uri gamesUrl = new UriBuilder(profileUrl).Uri.Append("games/?xml=1");
            string gamesXml = await webClient.DownloadStringTaskAsync(gamesUrl);

            XDocument root = XDocument.Parse(gamesXml);
            IEnumerable<SteamGame> gamesList = ParseGamesXml(root);

            return new SteamGamesList(gamesUrl, gamesList);
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
