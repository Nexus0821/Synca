using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRChat.Synca.API;

namespace VRChat.Synca.Cli
{
    internal class StandardConsoleInputHandler : AbstractConsoleInputHandler
    {
        public StandardConsoleInputHandler()
        {
            CreateArrowAndWaitForInput();
        }

        private void CreateArrowAndWaitForInput()
        {
            Console.Write("> ");
            OnInput(Console.ReadLine()!);
        }

        public override void OnInput(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                ProcessInput(ConsoleInput.Create(input));
            }
            else
            {
                Logger.Msg(ConsoleColor.DarkRed, "Cannot process an empty input");
                CreateArrowAndWaitForInput();
            }
        }

        protected override void ProcessInput(ConsoleInput input)
        {
            var command = input.inputParts[0];
            var arguments = input.inputParts.Skip(1).ToArray();

            if (command.value == "synca")
            {
                DirectoryInfo workingDirectory = new DirectoryInfo(Environment.CurrentDirectory);
                var syncaWorkingDirectory = workingDirectory.Parent!.FullName;
                var syncaPath = syncaWorkingDirectory + "/Synca.exe";

                if (arguments[0].value == "start")
                {
                    Logger.Msg(ConsoleColor.Green, "Starting Synca");

                    Process.Start(new ProcessStartInfo
                    {
                        FileName = syncaPath,
                        Arguments = "runas",
                        WorkingDirectory = syncaWorkingDirectory,
                        WindowStyle = ProcessWindowStyle.Normal,
                        UseShellExecute = true,
                        CreateNoWindow = false,
                    });
                }
                else if (arguments[0].value == "stop")
                {
                    var processes = Process.GetProcesses().ToList();
                    var syncaProcess = processes.FirstOrDefault(x => x.ProcessName.ToLower().Contains("synca") && !x.ProcessName.ToLower().Contains("cli"), null);

                    if (syncaProcess != null)
                    {
                        syncaProcess.Kill();
                        syncaProcess.Close();

                        Logger.Msg(ConsoleColor.Green, "Stopped Synca");
                    }
                }
            }

            CreateArrowAndWaitForInput();
        }
    }
}
