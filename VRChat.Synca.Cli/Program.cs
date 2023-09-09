using System.Drawing;
using VRChat.Synca.API;

namespace VRChat.Synca.Cli
{
    internal class Program
    {
        static void Main(string[] args)
        {
            MusicPlayer.Initialize<SimpleMusicPlayer>();

            for (; ; )
            {
            }
        }



        static object _lockObj = new object();
    }
}