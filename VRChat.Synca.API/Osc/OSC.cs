using BuildSoft.VRChat.Osc.Chatbox;
using Ewk.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRChat.Synca.API.Osc
{
    /// <summary>
    /// A wrapper for VRChat's <c>OSC (Open Sound Control)</c> system.
    /// </summary>
    public static class OSC
    {
        /// <summary>
        /// A wrapper for VRChat's OSC Chatbox.
        /// </summary>
        public static class Chatbox
        {
            /// <summary>
            /// Sets if the client is typing in the chatbox.
            /// </summary>
            /// <param name="typing">Is the user typing?</param>
            public static void SetTyping(bool typing) => OscChatbox.SetIsTyping(typing);

            /// <summary>
            /// Sends a message to the VRChat chatbox.
            /// </summary>
            /// <param name="messageBuilder">The builder of this message/param>
            /// <param name="direct">Indicates whether the message shows in direct or UI.</param>
            /// <param name="complete">Indicates whether the message uses to trigger the notification SFX.</param>
            public static void Send(IOscChatboxMessageBuilder messageBuilder, bool direct, bool complete = false)
            {
                if (messageBuilder == null) return;
                if (string.IsNullOrEmpty(messageBuilder.Message)) return;
                OscChatbox.SendMessage(messageBuilder.Message, direct, complete);
                Logger.Msg(ConsoleColor.Blue, "Sent message to OSC!");
            }
        }
    }
}
