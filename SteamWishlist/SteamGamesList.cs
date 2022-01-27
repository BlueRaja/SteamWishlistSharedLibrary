using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamWishlist
{
    public class SteamGamesList : IEnumerable<SteamGame>
    {
        public IEnumerable<SteamGame> Games { get; }

        public SteamGamesList(IEnumerable<SteamGame> games)
        {
            Games = games.ToList();
        }

        public IEnumerator<SteamGame> GetEnumerator()
        {
            return Games.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
