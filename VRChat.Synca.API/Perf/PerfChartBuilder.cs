using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRChat.Synca.API.Perf
{
    public class PerfBlock
    {
        string name;
        Stopwatch stopwatch;

        public static PerfBlock Create(string name)
        {
            return new PerfBlock
            {
                name = name,
                stopwatch = new Stopwatch()
            };
        }

        public string Name => name;
        public Stopwatch Stopwatch => stopwatch;
    }

    public interface IPerfChatBuilder
    {
        IPerfChatBuilder Start(string block);
        IPerfChatBuilder Stop(string block);
        StringBuilder Build();
        void BuildAndSave();

        bool Finished { get; }
    }

    public sealed class PerfChartBuilder : IPerfChatBuilder
    {
        string name;
        Dictionary<string, PerfBlock> perfBlocks = new Dictionary<string, PerfBlock>();

        public PerfChartBuilder(string name)
        {
            this.name = name;
        }

        public IPerfChatBuilder Start(string block)
        {
            if (perfBlocks.ContainsKey(block))
            {
                Logger.Msg(ConsoleColor.Red, "Cannot Start(): because a PerfBlock of this name already exists! Name = " + block);
                return this;
            }

            var perfBlock = PerfBlock.Create(block);
            perfBlock.Stopwatch.Start();

            perfBlocks.Add(block, perfBlock);
            return this;
        }

        public IPerfChatBuilder Stop(string block)
        {
            if (!perfBlocks.ContainsKey(block))
            {
                Logger.Msg(ConsoleColor.Red, "Cannot Start(): because a PerfBlock of this name does not exist! Name = " + block);
                return this;
            }
            var perfBlock = perfBlocks[block];
            perfBlock.Stopwatch.Stop();
            return this;
        }

        public StringBuilder Build()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(string.Format("Performance chart of '{0}'", name));
            foreach (var kvp in perfBlocks)
            {
                stringBuilder.AppendLine(string.Format("[{0} ({1} ms / {2} ticks)]'", kvp.Key, kvp.Value.Stopwatch.ElapsedMilliseconds, kvp.Value.Stopwatch.ElapsedTicks));
            }
            return stringBuilder;
        }

        public void BuildAndSave()
        {
            var sb = Build();
            var fileName = string.Format("perf chart {0}.txt", name);

            var writeAllTextResult = FileSystem.WriteAllText(Path.Combine(Paths.Performance, fileName), sb.ToString());
            if (writeAllTextResult.code != FileOperationErrorCode.Success)
                Logger.Msg(ConsoleColor.Red, string.Format("({0}) Failed to WriteAllText", writeAllTextResult.code));
        }

        public bool Finished => perfBlocks.Values.All(x => !x.Stopwatch.IsRunning);
    }
}
