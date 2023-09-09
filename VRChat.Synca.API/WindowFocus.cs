using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace VRChat.Synca.API
{
    public static class WindowFocus
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        public static Window GetFocusedWindow() => new Window(GetForegroundWindow());
    }

    public struct Window
    {
        private IntPtr _handle;
        public Window(IntPtr handle) { _handle = handle; }

        public int GetProcessId()
        {
            GetWindowThreadProcessId(_handle, out var processId);
            return (int)processId;
        }

        public string GetProcessName()
        {
            var processId = GetProcessId();
            var process = Process.GetProcessById(processId);
            return process.ProcessName;
        }

        public string GetWindowName()
        {
            const int nChars = 256;
            var buff = new StringBuilder(nChars);
            GetWindowText(_handle, buff, nChars);
            return buff.ToString();
        }

        public TimeSpan GetTimeOpen()
        {
            var processId = GetProcessId();
            var process = Process.GetProcessById(processId);
            return DateTime.Now - process.StartTime;
        }

        // Import necessary methods from user32.dll
        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
    }
}
