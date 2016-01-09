using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CsQuery;

namespace SteamWishlist
{
    public class SteamWishlistRetriever
    {
        private readonly WebClient _webClient;

        public SteamWishlistRetriever()
        {
            _webClient = new WebClient { Proxy = null }; //See http://stackoverflow.com/questions/4415443 - ugh.;
        }

        public async Task<SteamGamesList> GetWishlist(string profileUrl)
        {
            Uri wishlistUrl = new UriBuilder(profileUrl).Uri.Append("wishlist");
            string wishlistHtml = await _webClient.DownloadStringTaskAsync(wishlistUrl);
            CQ wishlistDom = CQ.Create(wishlistHtml);
            IEnumerable<SteamGame> gamesList = ParseWishlistPage(wishlistDom);
            return new SteamGamesList(wishlistUrl, gamesList);
        }

        private IEnumerable<SteamGame> ParseWishlistPage(CQ dom)
        {
            foreach (var wishlistNode in dom[".wishlistRow"])
            {
                CQ wishlistNodeDom = CQ.Create(wishlistNode);
                string name = wishlistNodeDom["h4.ellipsis"].Text();
                string url = wishlistNodeDom[".storepage_btn_ctn a"].Attr("href");
                yield return new SteamGame(name, url);
            }
        }
    }
}