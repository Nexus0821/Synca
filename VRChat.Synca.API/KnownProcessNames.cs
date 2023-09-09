using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRChat.Synca.API
{
    public static class KnownProcessNames
    {
        static Dictionary<string, string> _dict = new Dictionary<string, string>()
        {
            ["devenv"] = "Visual Studio",
            ["opera"] = "Opera GX",
            ["chrome"] = "Chrome"
        };

        public static string FormatName(string name)
        {
            bool foundKP = _dict.TryGetValue(name, out var value);
            if (!foundKP) value = name;

            return value;
        }
    }
}
