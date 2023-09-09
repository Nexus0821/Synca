using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRChat.Synca.API.Scheduler
{
    public struct RunnableQueueItem
    {
        public Runnable runnable;
        public object[] args;
    }

    public class StandardTaskDirector_ScheduleThread
    {
        Queue<RunnableQueueItem> allTasks = new Queue<RunnableQueueItem>();

        public void Run()
        {
            if (allTasks.Count > 0)
            {
                var nextRunnableItem = allTasks.Dequeue();
                Logger.Msg(ConsoleColor.Cyan, "Directing task " + nextRunnableItem.runnable.Id);
                nextRunnableItem.runnable.Run(nextRunnableItem.args);
            }
        }

        internal Runnable DirectTask(Runnable runnable, params object[] args)
        {
            allTasks.Enqueue(new RunnableQueueItem
            {
                runnable = runnable,
                args = args
            });
            return runnable;
        }

        internal RunnableThread DirectThread(RunnableThread runnableThread, params object[] args)
        {
            runnableThread.Run(args);
            return runnableThread;
        }

        public void Shutdown()
        {
            Logger.Msg(ConsoleColor.Cyan, "Shutting down TaskDirector schedule thread");
            for (int i = 0; i < allTasks.Count; i++)
                allTasks.Dequeue();
        }
    }

    public sealed class StandardTaskDirector : AbstractTaskDirector
    {
        StandardTaskDirector_ScheduleThread scheduleThread;
        bool running;

        int tasks;
        int threads;

        public StandardTaskDirector()
        {
            scheduleThread = new StandardTaskDirector_ScheduleThread();
            running = true;

            (new Thread(() =>
            {
                while (running) { scheduleThread.Run(); }
                scheduleThread.Shutdown();
            })).Start();
        }
        ~StandardTaskDirector()
        {
            running = false;
        }

        public override RunnableThread CreateThread(RunnableThread runnableThread, params object[] args)
        {
            threads++;
            return scheduleThread.DirectThread(runnableThread, args);
        }

        public override Runnable CreateTask(Action<object[]> action, params object[] args)
        {
            tasks++;
            return scheduleThread.DirectTask(new ImplicitRunnable(action), args);
        }

        public override Runnable CreateTask(Runnable runnable, params object[] args)
        {
            tasks++;
            return scheduleThread.DirectTask(runnable, args);
        }

        public override int TotalThreads => threads;
        public override int TotalTasks => tasks;
    }
}
