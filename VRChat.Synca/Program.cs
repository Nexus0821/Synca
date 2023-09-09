using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;

using VRChat.Synca.API;
using VRChat.Synca.API.Cached;
using VRChat.Synca.API.Scheduler;
using VRChat.Synca.API.Uptime;
using VRChat.Synca.API.Osc;
using VRChat.Synca.API.Perf;
using System.Management;

namespace VRChat.Synca
{
    public struct NexmMessageCycleItem
    {
        public double appearFor;
        public Func<IOscChatboxMessageBuilder> messageBuilder;
    }

    public static class Program
    {
        private static int currentMessageIndex = 0;
        private static long lastCacheClearTicks = 0;
        private static MEMORYSTATUSEX memoryStatus;

        private static List<NexmMessageCycleItem> messageBuildFuncs = new List<NexmMessageCycleItem>
        {
            new NexmMessageCycleItem
            {
                appearFor = 3,
                messageBuilder = () =>
                {
                    return new ChatboxMessageBuilder()
                                .AppendLine("Github: @Nexus0821")
                                .AppendLine("i like coding stuff");
                }
            },
            new NexmMessageCycleItem
            {
                appearFor = 3,
                messageBuilder =
                () =>
                {
                    double maxMemoryGB = Memory.ConvertBytesToGB((long)memoryStatus.ullTotalPhys);
                    double usedMemoryGB = Memory.ConvertBytesToGB((long)(memoryStatus.ullTotalPhys - memoryStatus.ullAvailPhys));

                    return new ChatboxMessageBuilder()
                                .AppendLine("CPU: " + CPU.Name)
                                .AppendLine("GPU: " + GPU.Name)
                                .AppendLine(string.Format("RAM: {0:F2}/{1:F2}GB               ", usedMemoryGB, Math.Round(maxMemoryGB)));
                },
            },
            new NexmMessageCycleItem
            {
                appearFor = 8,
                messageBuilder = () =>
                {
                    var isIdle = User.IsInactive(out var inactiveTime);
                    return new ChatboxMessageBuilder()
                                .AppendLine(string.Format("{0} ({1})", DateTime.Now.ToString("T"), TimeZoneInfo.Local.Id.Split(" ")[0]))
                                .AppendLine(string.Format("Tabbed Focus: {0}", KnownProcessNames.FormatName(WindowFocus.GetFocusedWindow().GetProcessName()).Truncate(14)))
                                .AppendLine(string.Format("Cache: {0:F2} GB ({1} cleared)", Memory.ConvertBytesToGB(CacheManager.Instance.ByteSize), CacheManager.Instance.LastClearedItemCount));
                }
            },
            new NexmMessageCycleItem
            {
                appearFor = 8,
                messageBuilder = () =>
                {
                    var defaultUptime = TimeSpan.FromSeconds(0);
                    var uptime = UptimeManager.GetTotalUptime();
                    return new ChatboxMessageBuilder()
                                .AppendLine("Statistics (all time)")
                                .AppendLine(string.Format("Uptime: {0}", (uptime.HasValue ? uptime.Value : defaultUptime).ToHumanReadableString()))
                                .AppendLine(string.Format("Cleared: {0:F2} GB ({1} items)", Memory.ConvertBytesToGB(CacheManager.TotalClearedBytes), CacheManager.TotalClearedItems))
                                .AppendLine(string.Format("Tasks Created: {0}", TaskDirector.Instance.TotalTasks));
                }
            }
        };

        private static void CleanupCache()
        {
            const string TOTAL = "TOTAL";
            const string CLEAR_CACHE_FROM_INST = "Clear Cache from Instance";
            const string GC_COLLECT = "GC.Collect";

            for (; ; )
            {
                var currentTime = DateTime.Now.Ticks;
                if ((currentTime - lastCacheClearTicks) > TimeSpan.FromSeconds(30).Ticks)
                {
                    PerfChartBuilder cleanupBuilder = new PerfChartBuilder("Cleanup Cache Task");

                    cleanupBuilder.Start(TOTAL);
                    lastCacheClearTicks = currentTime;

                    cleanupBuilder.Start(CLEAR_CACHE_FROM_INST);
                    CacheManager.Instance.ClearCache();
                    cleanupBuilder.Stop(CLEAR_CACHE_FROM_INST);

                    cleanupBuilder.Start(GC_COLLECT);
                    GC.Collect();
                    cleanupBuilder.Stop(GC_COLLECT);
                    cleanupBuilder.Stop(TOTAL);
                    cleanupBuilder.BuildAndSave();
                }
            }
        }

        private static async Task Main(string[] args)
        {
            const string TITLE = "Synca (VRChat OSC)";
            const string TOTAL = "TOTAL";
            const string SUB_AD_EVENTS = "Subscribe to AppDomain Events";
            const string INIT = "Initialize all Core Systems";
            const string START_TASKS = "Start main tasks - StandardTaskDirector";

            Console.Title = TITLE;

            PerfChartBuilder programChartBuilder = new PerfChartBuilder("Program");
            programChartBuilder.Start(TOTAL);
            programChartBuilder.Start(SUB_AD_EVENTS);
            AppDomain.CurrentDomain.UnhandledException += ((sender, args) =>
            {
                Directory.CreateDirectory("crashes");
                string errorLogName = string.Format("error_log_{0}.log", DateTime.Now.ToString("T").Replace(":", "_").Replace(" ", "_"));
                FileSystem.WriteAllText("crashes\\" + errorLogName, args.ExceptionObject.ToString());
            });
            programChartBuilder.Stop(SUB_AD_EVENTS);

            programChartBuilder.Start(INIT);
            Paths.Initialize();

            UptimeManager.Initialize();
            SyncaCacheManager.Initialize();

            TaskDirector.Initialize(new StandardTaskDirector());

            CacheManager.Initialize(new WindowsVRChatCache());
            programChartBuilder.Stop(INIT);

            programChartBuilder.Start(START_TASKS);
            TaskDirector.Instance.CreateTask((object[] args) => { new Thread(() => CleanupCache()).Start(); });
            programChartBuilder.Stop(START_TASKS);
            programChartBuilder.Stop(TOTAL);

            programChartBuilder.BuildAndSave();

            Logger.Msg(ConsoleColor.Blue, "Initializing MEMORYSTATUSEX");
            memoryStatus = new MEMORYSTATUSEX();
            memoryStatus.dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));

            Logger.Msg(ConsoleColor.Magenta, "Waiting for VRChat...");
            await TaskEx.WaitUntil(() => Process.GetProcesses().Any(x => x.ProcessName.ToLower().Contains("vrchat")));

            for (; ; )
            {
                Memory.GlobalMemoryStatusEx(ref memoryStatus);

                var chatboxMessageBuilder = _GetNextOscChatboxMessage(out var appearFor);
                if (chatboxMessageBuilder == null)
                {
                    Logger.Msg(ConsoleColor.Red, "Cannot submit message because the OscChatboxMessageBuilder is null! Index = " + currentMessageIndex);
                    return;
                }

                TaskDirector.Instance.CreateTask((args) => OSC.Chatbox.Send(chatboxMessageBuilder, true));
                await Task.Delay((int)(appearFor * 1000));
            }
        }

        private static IOscChatboxMessageBuilder _GetNextOscChatboxMessage(out double appearFor)
        {
            var messageCycle = messageBuildFuncs[currentMessageIndex];

            currentMessageIndex++;
            if (currentMessageIndex > messageBuildFuncs.IndexOf(messageBuildFuncs.Last()))
                currentMessageIndex = 0; // reset index

            appearFor = messageCycle.appearFor;
            return messageCycle.messageBuilder.Invoke();
        }
    }
}