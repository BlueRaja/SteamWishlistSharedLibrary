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
        public async Task<IEnumerable<SteamGame>> GetWishlist(string profileUrl)
        {
            Uri wishlistUrl = new Uri(profileUrl).Append("wishlist");
            using(WebClient client = new WebClient())
            {
                string wishlistHtml = await client.DownloadStringTaskAsync(wishlistUrl);
                CQ wishlistDom = CQ.Create(wishlistHtml);
                return ParseWishlistPage(wishlistDom);
            }
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