using System;

namespace Sequence.Boilerplates
{
    public static class TimeUtils
    {
        public static int GetTimestampSecondsNow()
        {
            return (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }
        
        public static string FormatRemainingTime(int totalSeconds)
        {
            if (totalSeconds <= 0)
                return "0s";

            var days = totalSeconds / (24 * 60 * 60);
            var hours = (totalSeconds % (24 * 60 * 60)) / (60 * 60);
            var minutes = (totalSeconds % (60 * 60)) / 60;
            var seconds = totalSeconds % 60;

            if (days > 0)
                return $"{days}d {hours}h";
            if (hours > 0)
                return $"{hours}h {minutes}m";
            if (minutes > 0)
                return $"{minutes}m {seconds}s";
            
            return $"{seconds}s";
        }
        
        public static int ConvertDateTimeToSeconds(string dateTimeString)
        {
            var dateTime = DateTime.Parse(dateTimeString, null, System.Globalization.DateTimeStyles.RoundtripKind);
            return (int)((DateTimeOffset)dateTime).ToUnixTimeSeconds();
        }
    }
}