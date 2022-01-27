using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SteamWishlist
{
    public class SteamWishlistRetriever
    {
        private readonly WebClient _webClient;

        public SteamWishlistRetriever()
        {
            _webClient = new WebClient { Proxy = null }; //See http://stackoverflow.com/questions/4415443 - ugh.;
        }

        public async Task<SteamGamesList> GetWishlist(string userIdentifier)
        {
            int currentPage = 0;
            List<SteamGame> steamGames = new List<SteamGame>();
            while(true)
            {
                string wishlistUrl = $"https://store.steampowered.com/wishlist/{userIdentifier}/wishlistdata/?p={currentPage}&v=103";
                string wishlistJson = await _webClient.DownloadStringTaskAsync(wishlistUrl);
                if (wishlistJson == "[]")
                    break;
                steamGames.AddRange(ParseWishlistPage(wishlistJson));
                currentPage++;
            }
            
            return new SteamGamesList(steamGames);
        }

        private class JsonGameInfo
        {
            public string name;
        }

        private IEnumerable<SteamGame> ParseWishlistPage(string wishlistJson)
        {
            var gamesList = JsonConvert.DeserializeObject<Dictionary<string, JsonGameInfo>>(wishlistJson);
            foreach (string appIdStr in gamesList.Keys)
            {
                long appId = long.Parse(appIdStr);
                yield return new SteamGame(appId, gamesList[appIdStr].name);
            }
        }
    }
}