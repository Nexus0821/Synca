using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRChat.Synca.API
{
    public static class Logger
    {
        public static void Msg(ConsoleColor color, string msg)
        {
            InternalMsg(color, msg);
        }

        public static void Msg(ConsoleColor color, string msg, params object[] args)
        {
            InternalMsg(color, string.Format(msg, args));
        }

        private static void InternalMsg(ConsoleColor color, string msg)
        {
            if (OnLogMessage != null) OnLogMessage(color, msg);

            lock (_lockObj)
            {
                Console.Write("[");
                Console.ForegroundColor = color;
                Console.Write(DateTime.Now.ToString("T"));
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("] " + msg + "\n");
            }
        }

        public static event Action<ConsoleColor, string> OnLogMessage;
        private static readonly object _lockObj = new();
    }
}
