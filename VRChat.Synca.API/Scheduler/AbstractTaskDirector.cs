using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRChat.Synca.API.Scheduler
{
    public interface ITaskDirector
    {
        RunnableThread CreateThread(RunnableThread runnableThread, params object[] args);

        Runnable CreateTask(Action<object[]> action, params object[] args);
        Runnable CreateTask(Runnable runnable, params object[] args);

        int TotalThreads { get; }
        int TotalTasks { get; }
    }

    public abstract class AbstractTaskDirector : ITaskDirector
    {
        public abstract RunnableThread CreateThread(RunnableThread runnableThread, params object[] args);

        public abstract Runnable CreateTask(Action<object[]> action, params object[] args);
        public abstract Runnable CreateTask(Runnable runnable, params object[] args);

        public abstract int TotalThreads { get; }
        public abstract int TotalTasks { get; }
    }
}
