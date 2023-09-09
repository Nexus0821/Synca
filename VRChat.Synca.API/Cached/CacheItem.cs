using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRChat.Synca.API.Cached
{
    public struct CacheItem
    {
        public static readonly CacheItem Invalid = new CacheItem(CacheItemType.Invalid, new byte[0], 0);

        public CacheItem(CacheItemType itemType, byte[] fileData, long byteSize)
        {
            this.itemType = itemType;
            this.data = fileData;
            this.byteSize = byteSize;
        }

        public CacheItemType itemType;
        public byte[] data;
        public long byteSize;
    }
}
