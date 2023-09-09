using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRChat.Synca.API.Cached
{
    public struct CacheFolder
    {
        public CacheFolder(string path)
        {
            int creationCount = 0;
            cacheSubfolders = new Dictionary<string, CacheData>();
            var allDirectoriesResult = FileSystem.GetDirectories(path, "*");
            foreach (var dirStr in allDirectoriesResult.code == FileOperationErrorCode.Success
                                                                    ? allDirectoriesResult.GetData<string[]>("result")
                                                                    : Array.Empty<string>())  // loop through all cache
            {                                       // root folders
                List<FileInfo> allFilesInfos = new List<FileInfo>();
                var getDirectoryInfoResult = FileSystem.GetDirectoryInfo(dirStr);
                if (getDirectoryInfoResult.code != FileOperationErrorCode.Success) return;

                var dirInfo = getDirectoryInfoResult.GetData<DirectoryInfo>("result");

                var allFilesResult = FileSystem.GetFiles(dirStr, "*", true);
                foreach (var fileStr in allFilesResult.code == FileOperationErrorCode.Success
                                                                    ? allFilesResult.GetData<string[]>("result")
                                                                    : Array.Empty<string>())
                {

                    var getFileInfoResult = FileSystem.GetFileInfo(fileStr);
                    if (getFileInfoResult.code == FileOperationErrorCode.Success)
                        allFilesInfos.Add(getFileInfoResult.GetData<FileInfo>("result"));
                }

                var __data = allFilesInfos.FirstOrDefault(x => x.Name == "__data", null);
                var __info = allFilesInfos.FirstOrDefault(x => x.Name == "__info", null);
                var __lock = allFilesInfos.FirstOrDefault(x => x.Name == "__lock", null);

                if (__data == null || __info == null) { }
                else
                {
                    creationCount++;
                    var cacheId = new CacheId(dirInfo.Name, __data!.Directory!.Name);

                    var dataResult = FileSystem.ReadAllBytes(__data!.FullName);
                    var infoResult = FileSystem.ReadAllBytes(__info!.FullName);
                    cacheSubfolders.Add(cacheId, new CacheData
                    {
                        cacheId = cacheId,

                        __data = new CacheItem(
                            CacheItemType.Data,
                            dataResult.code == FileOperationErrorCode.Success
                                ? dataResult.GetData<byte[]>("result")
                                : Array.Empty<byte>(),
                            __data.Length),

                        __info = new CacheItem(
                            CacheItemType.Info,
                            infoResult.code == FileOperationErrorCode.Success
                                ? infoResult.GetData<byte[]>("result")
                                : Array.Empty<byte>(),
                            __info.Length),

                        isLocked = __lock != null
                    });
                }
            }

            Logger.Msg(ConsoleColor.Green, string.Format("Created {0} cache entries", creationCount));
        }

        public CacheData GetCacheData(string cacheId)
        {
            if (!cacheSubfolders.ContainsKey(cacheId))
                throw new InvalidOperationException("Invalid CacheId!");

            return cacheSubfolders[cacheId];
        }

        public long GetTotalByteSize()
        {
            long byteSize = 0;
            foreach (var cacheData in cacheSubfolders.Values)
            {
                if (cacheData.__data != null) byteSize += cacheData.__data.Value.byteSize;
                if (cacheData.__info != null) byteSize += cacheData.__info.Value.byteSize;
            }
            return byteSize;
        }

        public Dictionary<string, CacheData> cacheSubfolders;
    }
}
