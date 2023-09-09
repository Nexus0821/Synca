using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRChat.Synca.API
{
    public class UserToken
    {
        public UserToken(string token)
        {
            _token = token;
        }

        public string Token => _token;
        private string _token;
    }

    public class Author
    {
        private string _name;
        private string _bio;
        private string _userId;
    }

    public class Song
    {
        private string _name;
        private Author _author;
        private TimeSpan _length;
    }

    public interface IMusicPlayer
    {
        void Initialize();
        void SetupSongFolder(string path);
    }

    public class SimpleMusicPlayer : IMusicPlayer
    {
        public void Initialize()
        {
        }

        public void SetupSongFolder(string path)
        {

        }
    }

    public static class MusicPlayer
    {
        public static void Initialize<TMusicPlayer>() where TMusicPlayer : IMusicPlayer
        {
            if (instance != null)
                throw new InvalidOperationException("Cannot initialize when a MusicPlayer is already initialized!");

            instance = Activator.CreateInstance<TMusicPlayer>();
            instance.Initialize();
        }

        public static IMusicPlayer Inst => instance;
        private static IMusicPlayer instance;
    }
}
