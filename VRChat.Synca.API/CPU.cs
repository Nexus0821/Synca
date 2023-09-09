using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Management;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VRChat.Synca.API
{
    public record CpuDevice(string manufacturer,
                            string friendlyName,
                            string deviceId);

    public static class CPU
    {
        public static string Name => GetProcessorName();
        private static Regex coreAmountRegex = new Regex("[0-9]+-Core");

        private static CpuDevice SanitizeDevice(ref CpuDevice device)
        {
            // if our dev name contains 6-Core or anything similar
            // we can just remove it to shorten our dev name
            var coreAmountMatch = coreAmountRegex.Match(device.friendlyName);
            if (coreAmountMatch != null) // we got a match
            {
                string friendlyName = device.friendlyName.Split(coreAmountMatch.Value)[0];
                device = new CpuDevice(device.manufacturer, friendlyName, device.deviceId);
            }

            return device;
        }

        public static string GetProcessorName()
        {
            var processorNames = GetProcessorNames();
            if (processorNames == null || processorNames!.Count == 0) return "No CPU found";

            var device = processorNames.Find(x => x.deviceId == "CPU0") ?? new CpuDevice("null", "null", "null");
            SanitizeDevice(ref device);

            return device.friendlyName;
        }

        public static List<CpuDevice>? GetProcessorNames()
        {
            List<CpuDevice> retval = new List<CpuDevice>();
            using (ManagementObjectSearcher mos = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Processor"))
            {
                foreach (ManagementObject mo in mos.Get())
                {
                    retval.Add(new CpuDevice((string)mo["Manufacturer"]!, (string)mo["Name"]!, (string)mo["DeviceID"]!));
                }
            }
            return retval;
        }
    }
}
