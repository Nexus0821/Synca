using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRChat.Synca.API.Uptime
{
    public static class UptimeManager
    {
        public static void Initialize()
        {
            if (stopwatch != null)
            {
                Logger.Msg(ConsoleColor.Red, "Cannot initialize UptimeManager because one is already present!");
                return;
            }

            stopwatch = Stopwatch.StartNew();
        }

        public static TimeSpan? GetTotalUptime()
        {
            if (stopwatch == null)
            {
                Logger.Msg(ConsoleColor.Red, "Cannot GetTotalUptime(): because the stopwatch is not initialized!");
                return null;
            }

            return stopwatch.Elapsed;
        }

        private static Stopwatch stopwatch;
    }
}
