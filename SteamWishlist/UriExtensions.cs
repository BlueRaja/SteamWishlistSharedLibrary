using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamWishlist
{
    public static class UriExtensions
    {
        /// <summary>
        /// Append a path to a URL
        /// Taken from http://stackoverflow.com/a/7993235/238419
        /// </summary>
        public static Uri Append(this Uri uri, params string[] paths)
        {
            return new Uri(paths.Aggregate(uri.AbsoluteUri, (current, path) => String.Format("{0}/{1}", current.TrimEnd('/'), path.TrimStart('/'))));
        }
    }
}
