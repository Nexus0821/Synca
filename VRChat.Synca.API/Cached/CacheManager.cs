using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRChat.Synca.API.Cached
{
    public class CacheStatistics
    {
        public int totalClearedItems;
        public long totalClearedBytes;
    }

    public static class CacheManager
    {
        const string STATKEY = "vrchat_cache_stats";

        public static void Initialize(VRChatCacheBase cacheBase)
        {
            if (SyncaCacheManager.Exists(STATKEY))
            {
                var stats = SyncaCacheManager.Get<CacheStatistics>(STATKEY);
                TotalClearedItems = stats.totalClearedItems;
                TotalClearedBytes = stats.totalClearedBytes;
            }

            if (_cacheBase != null)
                return;

            _cacheBase = cacheBase;
        }

        public static void ReportCacheClear(int count, long bytes)
        {
            TotalClearedItems += count;
            TotalClearedBytes += bytes;

            CacheStatistics cacheStatistics = new CacheStatistics()
            {
                totalClearedItems = TotalClearedItems,
                totalClearedBytes = TotalClearedBytes
            };
            if (!SyncaCacheManager.Exists(STATKEY))
                SyncaCacheManager.Add(STATKEY, cacheStatistics);
            else
                SyncaCacheManager.Set(STATKEY, cacheStatistics);
        }

        public static int TotalClearedItems { get; private set; }
        public static long TotalClearedBytes { get; private set; }

        public static VRChatCacheBase Instance => _cacheBase;
        private static VRChatCacheBase _cacheBase;
    }
}
