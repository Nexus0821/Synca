using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VRChat.Synca.API.Scheduler
{
    public interface IRunnable
    {
        void Run(params object[] args);
        string Id { get; }
    }

    public class ImplicitRunnable : Runnable
    {
        Action<object[]> runAction;

        public ImplicitRunnable(Action<object[]> runAction)
        {
            this.runAction = runAction;
        }

        public override void Run(params object[] args)
        {
            if (runAction == null)
            {
                Logger.Msg(ConsoleColor.DarkRed, "Cannot run implicit runnable because the run action is null!");
                return;
            }    

            runAction(args);
        }
    }

    public abstract class Runnable : IRunnable
    {
        string id = Guid.NewGuid().ToString();

        public static implicit operator Runnable(Action<object[]> runAction)
        {
            return new ImplicitRunnable(runAction);
        }

        public abstract void Run(params object[] args);
        public string Id => id;
    }

    public abstract class RunnableThread : Runnable
    {
        public override void Run(params object[] args)
        {
            (new Thread(() => { ThreadStart(args); })
            {
                Name = this.GetType().Name
            }).Start();
        }

        public abstract void ThreadStart(object[] args);
    }
}
