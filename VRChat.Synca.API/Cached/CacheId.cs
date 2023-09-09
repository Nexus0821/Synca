using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRChat.Synca.API.Cached
{
    public struct CacheId : IEquatable<CacheId>
    {
        public CacheId(string cacheFolder, string cacheSubfolder)
        {
            s1 = cacheFolder;
            s2 = cacheSubfolder;
        }

        public bool Equals(CacheId other)
        {
            return s1 == other.s1 && s2 == other.s2;
        }

        public static implicit operator string(CacheId cacheId) => cacheId.ToString();
        public override string ToString()
        {
            return string.Format("{0}.{1}", s1, s2);
        }

        public string CacheFolder => s1;
        public string CacheSubfolder => s2;

        private string s1;
        private string s2;
    }
}
