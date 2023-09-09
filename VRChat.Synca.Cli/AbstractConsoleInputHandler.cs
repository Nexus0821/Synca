using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRChat.Synca.Cli
{
    public class ConsoleInput
    {
        public static ConsoleInput Create(string inputLine)
        {
            string[] parts = inputLine.Split(' ');
            ConsoleInputPart[] inputParts = new ConsoleInputPart[parts.Length];

            for (int i = 0; i < parts.Length; i++)
            {
                inputParts[i] = new ConsoleInputPart
                {
                    value = parts[i],
                    index = i
                };
            }

            return new ConsoleInput { inputParts = inputParts };
        }

        public override string ToString()
        {
            return string.Join(" ", (from x in inputParts
                                     select x.value));
        }

        public ConsoleInputPart[] inputParts; 
    }

    public class ConsoleInputPart
    {
        public string value;
        public int index;
    }

    public abstract class AbstractConsoleInputHandler
    {
        public abstract void OnInput(string input);
        protected abstract void ProcessInput(ConsoleInput input);
    }
}
