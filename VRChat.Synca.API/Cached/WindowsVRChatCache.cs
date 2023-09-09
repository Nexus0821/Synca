using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRChat.Synca.API.Cached
{
    public sealed class WindowsVRChatCache : VRChatCacheBase
    {
        static readonly string PATH = CreateCachePath();

        CacheFolder cacheFolder;
        int lastClearCount = 0;

        static string CreateCachePath()
        {
            return Path.Combine(Paths.VRChatAppData, "Cache-WindowsPlayer");
        }

        private void UpdateCache()
        {
            cacheFolder = new CacheFolder(PATH);
        }

        public override byte[] GetCache(string cacheId, CacheItemType itemType)
        {
            UpdateCache();
            var data = cacheFolder.GetCacheData(cacheId);
            if (itemType == CacheItemType.Data) return data.__data!.Value.data;
            if (itemType == CacheItemType.Info) return data.__info!.Value.data;

            Logger.Msg(ConsoleColor.Red, "No file available for " + itemType);
            return new byte[0];
        }

        public override void ClearCache()
        {
            if (PATH.IsNullOrEmpty())
            {
                Logger.Msg(ConsoleColor.DarkRed, "ERROR SWITCH: dropping clear task because the path to AppData is null!");
                return;
            }

            UpdateCache();
            List<string> markedForDeletion = new List<string>();
            foreach (var cacheSubfolder in cacheFolder.cacheSubfolders.Values)
            {
                if (!cacheSubfolder.isLocked)
                {
                    string cachedMfd = string.Format("{0}\\{1}", cacheSubfolder.cacheId.CacheFolder, cacheSubfolder.cacheId.CacheSubfolder);
                    Logger.Msg(ConsoleColor.Cyan, string.Format("Marking '{0}' for cache deletion", cachedMfd));
                    markedForDeletion.Add(cachedMfd);
                }
            }
            markedForDeletion.Remap((orgValue) => { return PATH + "\\" + orgValue; });

            int deletedItems = 0;
            long byteSize = 0;
            foreach (var mfdCacheSubfolder in markedForDeletion)
            {
                bool deletionConfirmed = false;
                var getAllFilesResult = FileSystem.GetFiles(mfdCacheSubfolder, "*", false);
                foreach (var fileStr in getAllFilesResult.code == FileOperationErrorCode.Success
                                                                    ? getAllFilesResult.GetData<string[]>("result")
                                                                    : Array.Empty<string>())
                {
                    var getFileInfoResult = FileSystem.GetFileInfo(fileStr);
                    if (getAllFilesResult.code == FileOperationErrorCode.Success)
                    {
                        var fileInfo = getFileInfoResult.GetData<FileInfo>("result");
                        byteSize += fileInfo.Length;


                        var deleteCacheFileResult = FileSystem.DeleteFile(fileInfo.FullName!);
                        if (deleteCacheFileResult.code == FileOperationErrorCode.Success)
                        {
                            deletedItems++;
                            deletionConfirmed = true;
                        }
                    }

                    if (deletionConfirmed)
                    {
                        var deleteCacheDirectoryResult = FileSystem.DeleteDirectory(mfdCacheSubfolder);
                        if (deleteCacheDirectoryResult.code == FileOperationErrorCode.Success)
                            deletedItems++;
                    }
                }
            }
            lastClearCount = deletedItems;
            CacheManager.ReportCacheClear(deletedItems, byteSize);

            DeleteEmptyCaches();
            Logger.Msg(ConsoleColor.Cyan, string.Format("Deleted {0} item(s) from WindowsPlayer cache", deletedItems));
        }

        private void DeleteEmptyCaches()
        {
            foreach (var cacheSubfolder in cacheFolder.cacheSubfolders.Values)
            {
                string cachedMfd = PATH + string.Format("\\{0}", cacheSubfolder.cacheId.CacheFolder);
                DirectoryInfo info = new DirectoryInfo(cachedMfd);

                if (info.GetDirectories().Length == 0 && info.Parent != null) // we have no files
                    FileSystem.DeleteDirectory(info.FullName!);
            }
            UpdateCache();
        }

        public override int LastClearedItemCount => lastClearCount;

        public override long ByteSize
        {
            get
            {
                UpdateCache();
                return cacheFolder.GetTotalByteSize();
            }
        }

        public override CacheType CacheType => CacheType.WindowsPlayer;
    }
}
