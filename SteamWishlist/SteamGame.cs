﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamWishlist
{
    public struct SteamGame : IEquatable<SteamGame>
    {
        public SteamGame(string name, string url)
        {
            Url = url;
            Name = name;
        }

        public string Name { get; }
        public string Url { get; }

        #region Equality members (auto-generated by Resharper)
        public bool Equals(SteamGame other)
        {
            if(ReferenceEquals(null, other)) return false;
            if(ReferenceEquals(this, other)) return true;
            return string.Equals(Name, other.Name) && string.Equals(Url, other.Url);
        }

        public override bool Equals(object obj)
        {
            if(ReferenceEquals(null, obj)) return false;
            if(ReferenceEquals(this, obj)) return true;
            if(obj.GetType() != this.GetType()) return false;
            return Equals((SteamGame)obj);
        }

        public override int GetHashCode()
        {
            return ((Name?.GetHashCode() ?? 0) * 397) ^ (Url?.GetHashCode() ?? 0);
        }

        public static bool operator ==(SteamGame left, SteamGame right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(SteamGame left, SteamGame right)
        {
            return !Equals(left, right);
        }
        #endregion
    }
}
