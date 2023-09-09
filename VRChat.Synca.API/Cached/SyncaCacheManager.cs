using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRChat.Synca.API.Crypto;

namespace VRChat.Synca.API.Cached
{
    public class CacheEntry
    {
        public int id;
        public string key;
        public object value;
    }

    public static class SyncaCacheManager
    {
        static readonly string PATH = CreateCachePath();
        static string CreateCachePath() { return Path.Combine(Environment.CurrentDirectory, "synca_cache"); }

        public static void Initialize()
        {
            Directory.CreateDirectory(PATH); // make sure our dir exists! :)
            UpdateCache();
        }

        static void UpdateCache()
        {
            lock (lockObj)
            {
                cacheEntries.Clear();
                foreach (var fileStr in Directory.GetFiles(PATH, "*.cache"))
                {
                    var getFileInfoResult = FileSystem.GetFileInfo(fileStr);
                    if (getFileInfoResult.code == FileOperationErrorCode.Success)
                    {
                        FileInfo info = getFileInfoResult.GetData<FileInfo>("result");
                        string json = FileSystem.ReadAllText(info.FullName).GetData<string>("result");

                        if (!json.IsNullOrEmpty())
                            cacheEntries.Add(JsonConvert.DeserializeObject<CacheEntry>(json)!);
                    }
                }
            }
        }

        public static bool Exists(string key)
        {
            UpdateCache();
            return cacheEntries.Any(x => x.key == key);
        }

        public static void Add<T>(string key, T value)
        {
            UpdateCache();

            var cacheId = (key.GetHashCode() + value!.GetHashCode());
            var cacheEntry = new CacheEntry
            {
                id = cacheId,
                key = key,
                value = value
            };

            var fileName = string.Format("{0}.cache", Hashing.MD5(cacheId.ToString()));
            cacheEntries.Add(cacheEntry);

            FileSystem.WriteAllText(PATH + "\\" + fileName, JsonConvert.SerializeObject(cacheEntry));
        }

        public static void Set<T>(string key, T value) 
        { 
            UpdateCache();

            bool foundEntry = false;
            lock (lockObj)
            {
                foreach (var cacheEntry in cacheEntries)
                {
                    if (cacheEntry.key == key)
                    {
                        cacheEntry.value = value;
                        var fileName = string.Format("{0}.cache", Hashing.MD5(cacheEntry.id.ToString()));
                        FileSystem.WriteAllText(PATH + "\\" + fileName, JsonConvert.SerializeObject(cacheEntry));
                        foundEntry = true;
                    }
                }
            }

            if (foundEntry) return;

            Logger.Msg(ConsoleColor.Red, "(SyncaCacheManager) Cannot Set(): because there is no cache entry! Key = " + key);
            return;
        }

        public static void Remove(string key)
        {
            UpdateCache();

            var cacheEntry = cacheEntries.FirstOrDefault(x => x.key == key, null);
            if (cacheEntry == null)
            {
                Logger.Msg(ConsoleColor.Red, "(SyncaCacheManager) Cannot Remove(): because there is no cache entry! Key = " + key);
                return;
            }

            var fileName = string.Format("{0}.cache", Hashing.MD5(cacheEntry.id.ToString()));
            FileSystem.DeleteFile(PATH + "\\" + fileName);

            cacheEntries.Remove(cacheEntry);
        }

        public static T Get<T>(string key)
        {
            UpdateCache();

            var cacheEntry = cacheEntries.FirstOrDefault(x => x.key == key, null);
            if (cacheEntry == null)
            {
                Logger.Msg(ConsoleColor.Red, "(SyncaCacheManager) Cannot Get(): because there is no cache entry! Key = " + key);
                return default(T)!;
            }

            return JsonConvert.DeserializeObject<T>(((JObject)cacheEntry.value).ToString());
        }

        private static readonly object lockObj = new();
        private static List<CacheEntry> cacheEntries = new List<CacheEntry>();
    }
}
