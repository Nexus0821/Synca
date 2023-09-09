using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRChat.Synca.API.Osc
{
    public interface IOscChatboxMessageBuilder
    {
        IOscChatboxMessageBuilder Append(string text);
        IOscChatboxMessageBuilder AppendLine(string text);
        IOscChatboxMessageBuilder AppendFormat(string text, params object[] args);

        string Message { get; }
        int Length { get; }
    }

    public sealed class ChatboxMessageBuilder : IOscChatboxMessageBuilder
    {
        const int MAX_CHARS = 160;
        const int MAX_PADDING_CHARS = 50;
        private StringBuilder _stringBuilder = new StringBuilder();

        public IOscChatboxMessageBuilder Append(string text)
        {
            if (Length >= MAX_CHARS)
                return this;

            Length += text.Length;

            _stringBuilder.Append(text);
            return this;
        }

        public IOscChatboxMessageBuilder AppendLine(string text)
        {
            if (Length >= MAX_CHARS)
                return this;

            var message = text.PadRight(MAX_PADDING_CHARS, ' ');
            Length += message.Length;

            _stringBuilder.AppendLine(message);
            return this;
        }

        public IOscChatboxMessageBuilder AppendFormat(string text, params object[] args)
        {
            if (Length >= MAX_CHARS)
                return this;

            string message = string.Format(text, args);
            Length += message.Length;

            _stringBuilder.Append(message);
            return this;
        }

        public string Message => _stringBuilder.ToString();
        public int Length { get; private set; }
    }
}
