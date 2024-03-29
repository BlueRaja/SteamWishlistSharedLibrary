﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SteamWishlist
{
    public class SteamProfileUrlValidator
    {
        private readonly Regex _profileUrlRegex = new Regex(@"^(https?://)?(www\.)?(steamcommunity|steampowered)\.com/(profiles/\d+|id/[a-z0-9_]+)/?$", RegexOptions.IgnoreCase);
        public bool IsValidSteamProfileUrl(string profileUrl)
        {
            return _profileUrlRegex.IsMatch(profileUrl);
        }

        public string GetUserIdentifier(string profileUrl)
        {
            return _profileUrlRegex.Match(profileUrl).Groups[4].Value;
        }
    }
}
