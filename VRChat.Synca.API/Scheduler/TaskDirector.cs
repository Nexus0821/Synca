using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRChat.Synca.API.Scheduler
{
    public static class TaskDirector
    {
        public static void Initialize(AbstractTaskDirector taskDirector)
        {
            if (_taskDirector != null)
            {
                Logger.Msg(ConsoleColor.Red, "Cannot initialize TaskDirector because one is already present!");
                return;
            }

            _taskDirector = taskDirector;
        }

        public static AbstractTaskDirector Instance => _taskDirector;
        private static AbstractTaskDirector _taskDirector;
    }
}
