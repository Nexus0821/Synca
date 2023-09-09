using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace VRChat.Synca.API.Cached
{
    public enum CacheType
    {
        Invalid = 0,
        WindowsPlayer
    }

    public enum CacheItemType
    {
        Invalid = 0,
        Data,
        Info,
        Lock
    }

    public interface IVRChatCacheBase
    {
        byte[] GetCache(string cacheId, CacheItemType itemType);
        void ClearCache();

        int LastClearedItemCount { get; }
        long ByteSize { get; }
        CacheType CacheType { get; }
    }

    public abstract class VRChatCacheBase : IVRChatCacheBase
    {
        public abstract byte[] GetCache(string cacheId, CacheItemType itemType);
        public abstract void ClearCache();

        public abstract int LastClearedItemCount { get; }
        public abstract long ByteSize { get; }
        public abstract CacheType CacheType { get; }
    }
}
