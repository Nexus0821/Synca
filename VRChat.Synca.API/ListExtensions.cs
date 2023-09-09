using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRChat.Synca.API
{
    public static class ListExtensions
    {
        public static void Remap<T>(this List<T> list, Func<T, T> remapFunc)
        {
            for (int i = 0; i < list.Count; i++)
                list[i] = remapFunc(list[i]);
        }
    }
}
