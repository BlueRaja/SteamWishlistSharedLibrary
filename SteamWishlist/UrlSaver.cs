using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteamWishlist.Properties;

namespace SteamWishlist
{
    public static class UrlSaver
    {
        public static string MySteamProfile
        {
            get
            {
                return Settings.Default.MyProfileUrl;
            }
            set
            {
                Settings.Default.MyProfileUrl = value;
                Settings.Default.Save();
            }
        }

        private const string Seperator = ";-;";
        public static IEnumerable<string> TheirSteamProfiles
        {
            get
            {
                return Settings.Default.TheirProfileUrls?.Split(new[] {Seperator}, StringSplitOptions.RemoveEmptyEntries) ?? new string[] {};
            }
            set
            {
                Settings.Default.TheirProfileUrls = String.Join(Seperator, value.Select(o => o.Replace(Seperator, "")));
                Settings.Default.Save();
            }
        } 
    }
}
