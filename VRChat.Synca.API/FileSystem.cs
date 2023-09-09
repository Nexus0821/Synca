using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRChat.Synca.API
{
    public enum FileOperationErrorCode
    {
        Success,
        Error,
        Unapproved,
        UnhandledException
    }

    public class FileOperationResultBase { }

    public class FileOperationResult : FileOperationResultBase
    {
        public static FileOperationResult Success => new FileOperationResult(FileOperationErrorCode.Success, "");
        public static FileOperationResult Unapproved => new FileOperationResult(FileOperationErrorCode.Unapproved, "");
        public static FileOperationResult UnhandledException => new FileOperationResult(FileOperationErrorCode.UnhandledException, "");
        public static FileOperationResult Error => new FileOperationResult(FileOperationErrorCode.Error, "");

        public FileOperationResult(FileOperationErrorCode code, string path)
        {
            this.code = code;
            this.path = path;
            this.data = new Dictionary<string, object>();
        }

        public FileOperationResult ChangeCode(FileOperationErrorCode code)
        {
            this.code = code;
            return this;
        }

        public FileOperationResult ChangePath(string path)
        {
            this.path = path;
            return this;
        }

        public FileOperationResult AddData(string key, object obj)
        {
            if (data.ContainsKey(key))
                data[key] = obj;
            else data.Add(key, obj);

            return this;
        }

        public T GetData<T>(string key)
        {
            if (!this.data.ContainsKey(key))
                return default(T)!;
            return (T)data[key];
        }

        public FileOperationErrorCode code;
        public string path;
        public Dictionary<string, object> data;
    }

    public static class Paths
    {
        public static void Initialize()
        {
            FileSystem.AddApprovedPath(WorkingPath);
            FileSystem.AddApprovedPath(RoamingAppData);
            FileSystem.AddApprovedPath(LocalAppData);
            FileSystem.AddApprovedPath(LocalLowAppData);
            FileSystem.AddApprovedPath(VRChatAppData);
            FileSystem.AddApprovedPath(Crashes);
            FileSystem.AddApprovedPath(Performance);
        }

        public static string WorkingPath => Environment.CurrentDirectory;
        public static string RoamingAppData => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static string LocalAppData => RoamingAppData.Replace("\\Roaming", "\\Local");
        public static string LocalLowAppData => RoamingAppData.Replace("\\Roaming", "\\LocalLow");
        public static string VRChatAppData => Path.Combine(LocalLowAppData, "VRChat", "VRChat");
        public static string Crashes => Path.Combine(WorkingPath, "crashes");
        public static string Performance => Path.Combine(WorkingPath, "performance");
    }

    public static class FileSystem
    {
        static List<string> approvedPaths = new List<string>();

        public static void AddApprovedPath(string path) { approvedPaths.Add(path); }
        public static void RemoveApprovedPath(string path) { approvedPaths.Remove(path); }

        public static FileOperationResult GetFileInfo(string path)
        {
            if (!approvedPaths.Any(x => path.StartsWith(x)))
                return FileOperationResult.Unapproved.ChangePath(path);

            try
            {
                var result = new FileInfo(path);
                if (result.Exists)
                    return FileOperationResult.Success.AddData("result", result)
                                                        .ChangePath(path);
                else
                    return FileOperationResult.Error.AddData("error", "No file exists")
                                                        .ChangePath(path);
            }
            catch (Exception ex)
            {
                return FileOperationResult.UnhandledException.AddData("exception", ex)
                                                                .ChangePath(path);
            }
        }

        public static FileOperationResult GetDirectoryInfo(string path)
        {
            if (!approvedPaths.Any(x => path.StartsWith(x)))
                return FileOperationResult.Unapproved.ChangePath(path);

            try
            {
                var result = new DirectoryInfo(path);
                if (result.Exists)
                    return FileOperationResult.Success.AddData("result", result)
                                                        .ChangePath(path);
                else
                    return FileOperationResult.Error.AddData("error", "No file exists")
                                                        .ChangePath(path);
            }
            catch (Exception ex)
            {
                return FileOperationResult.UnhandledException.AddData("exception", ex)
                                                                .ChangePath(path);
            }
        }

        public static FileOperationResult ReadAllBytes(string path)
        {
            if (!approvedPaths.Any(x => path.StartsWith(x)))
                return FileOperationResult.Unapproved.ChangePath(path);

            try
            {
                var result = File.ReadAllBytes(path);
                return FileOperationResult.Success.AddData("result", result)
                                                    .ChangePath(path);
            }
            catch (Exception ex)
            {
                return FileOperationResult.UnhandledException.AddData("exception", ex)
                                                                .ChangePath(path);
            }
        }

        public static FileOperationResult ReadAllText(string path)
        {
            if (!approvedPaths.Any(x => path.StartsWith(x)))
                return FileOperationResult.Unapproved.ChangePath(path);

            try
            {
                var result = File.ReadAllText(path);
                return FileOperationResult.Success.AddData("result", result)
                                                    .ChangePath(path);
            }
            catch (Exception ex)
            {
                return FileOperationResult.UnhandledException.AddData("exception", ex)
                                                                .ChangePath(path);
            }
        }

        public static FileOperationResult WriteAllText(string path, string? contents)
        {
            if (!approvedPaths.Any(x => path.StartsWith(x)))
                return FileOperationResult.Unapproved.ChangePath(path);

            try
            {
                File.WriteAllText(path, contents);
                return FileOperationResult.Success.ChangePath(path);
            }
            catch (Exception ex)
            {
                return FileOperationResult.UnhandledException.AddData("exception", ex)
                                                                .ChangePath(path);
            }
        }

        public static FileOperationResult DeleteFile(string path)
        {
            if (!approvedPaths.Any(x => path.StartsWith(x)))
                return FileOperationResult.Unapproved.ChangePath(path);

            try
            {
                File.Delete(path);
                return FileOperationResult.Success.ChangePath(path);
            }
            catch (Exception ex)
            {
                return FileOperationResult.UnhandledException.AddData("exception", ex)
                                                                .ChangePath(path);
            }
        }

        public static FileOperationResult DeleteDirectory(string path)
        {
            if (!approvedPaths.Any(x => path.StartsWith(x)))
                return FileOperationResult.Unapproved.ChangePath(path);

            try
            {
                Directory.Delete(path);
                return FileOperationResult.Success.ChangePath(path);
            }
            catch (Exception ex)
            {
                return FileOperationResult.UnhandledException.AddData("exception", ex)
                                                                .ChangePath(path);
            }
        }

        public static FileOperationResult GetDirectories(string path, string searchPattern = "*", bool recursive = false)
        {
            if (!approvedPaths.Any(x => path.StartsWith(x)))
                return FileOperationResult.Unapproved.ChangePath(path);

            try
            {
                var result = Directory.GetDirectories(path, searchPattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
                return FileOperationResult.Success.AddData("result", result)
                                                    .ChangePath(path);
            }
            catch (Exception ex)
            {
                return FileOperationResult.UnhandledException.AddData("exception", ex)
                                                                .ChangePath(path);
            }
        }

        public static FileOperationResult GetFiles(string path, string searchPattern = "*", bool recursive = false)
        {
            if (!approvedPaths.Any(x => path.StartsWith(x)))
                return FileOperationResult.Unapproved.ChangePath(path);

            try
            {
                var result = Directory.GetFiles(path, searchPattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
                return FileOperationResult.Success.AddData("result", result)
                                                    .ChangePath(path);
            }
            catch (Exception ex)
            {
                return FileOperationResult.UnhandledException.AddData("exception", ex)
                                                                .ChangePath(path);
            }
        }
    }
}
