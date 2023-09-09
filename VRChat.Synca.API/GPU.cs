using OpenHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace VRChat.Synca.API
{
    public static class GPU
    {
        public static string Name => GetHardwareName();

        public static string GetHardwareName()
        {
            var hardwareNames = GetHardwareNames();
            return hardwareNames == null ? "No GPU (integrated graphics)" : hardwareNames.Last();
        }

        public static List<string>? GetHardwareNames()
        {
            List<string> hardwareNames = new List<string>();
            using (var searcher = new ManagementObjectSearcher("select * from Win32_VideoController"))
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    hardwareNames.Add((string)obj["Name"]!);
                }
            }
            return hardwareNames.Count == 0 ? null : hardwareNames;
        }
    }
}
