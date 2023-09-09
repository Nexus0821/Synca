using System.Drawing;
using VRChat.Synca.API;

namespace VRChat.Synca.Cli
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            _ = Task.Run(() =>
            {
                new StandardConsoleInputHandler();
            });

            await Task.Delay(-1);
        }

        static object _lockObj = new object();
    }
}