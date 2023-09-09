using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRChat.Synca.API
{
    public static class TimeSpanExtensions
    {
        public static string ToHumanReadableString(this TimeSpan timeSpan)
        {
            List<string> concatParts = new List<string>();

            if (timeSpan.Days > 365)
            {
                int years = timeSpan.Days / 365;
                concatParts.Add($"{years}y");
                timeSpan = timeSpan.Subtract(TimeSpan.FromDays(years * 365));
            }

            if (timeSpan.Days > 30)
            {
                int months = timeSpan.Days / 30;
                concatParts.Add($"{months}mm");
                timeSpan = timeSpan.Subtract(TimeSpan.FromDays(months * 30));
            }

            if (timeSpan.Days > 0)
                concatParts.Add($"{timeSpan.Days}d");

            if (timeSpan.Hours > 0)
                concatParts.Add($"{timeSpan.Hours}h");

            if (timeSpan.Minutes > 0)
                concatParts.Add($"{timeSpan.Minutes}m");

            if (timeSpan.Seconds > 0)
                concatParts.Add($"{timeSpan.Seconds}s");

            if (concatParts.Count == 0)
                return "0 seconds";

            return string.Join(", ", concatParts);
        }
    }
}
