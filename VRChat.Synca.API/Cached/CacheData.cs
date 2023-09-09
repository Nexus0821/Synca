using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRChat.Synca.API.Cached
{
    public struct CacheData
    {
        public CacheId cacheId;
        public CacheItem? __data;
        public CacheItem? __info;
        public bool isLocked;
    }
}
