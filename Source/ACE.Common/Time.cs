using System;

namespace ACE.Common
{
    public class Time
    {
        private static readonly DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        public static double GetUnixTime()
        {
            return GetUnixTime(DateTime.UtcNow);
        }

        public static double GetUnixTime(DateTime dateTime)
        {
            TimeSpan span = (dateTime - unixEpoch);

            return span.TotalSeconds;
        }

        public static double GetFutureUnixTime(double seconds)
        {
            TimeSpan span = (DateTime.UtcNow.AddSeconds(seconds) - unixEpoch);

            return span.TotalSeconds;
        }

        public static DateTime GetDateTimeFromTimestamp(double timestamp)
        {
            return unixEpoch.AddSeconds(timestamp);
        }

        /// <summary>
        /// Returns a human-readable string describing how much time remains from <paramref name="startTime"/> + <paramref name="duration"/>.
        /// Example: "4 hours 32 minutes" or the <paramref name="expiredText"/> if already elapsed.
        /// </summary>
        public static string GetTimeRemainingString(DateTime startTime, TimeSpan duration, string expiredText = "expired")
        {
            var remaining = (startTime + duration) - DateTime.UtcNow;

            if (remaining <= TimeSpan.Zero)
                return expiredText;

            if (remaining.TotalHours >= 1)
                return $"{(int)remaining.TotalHours}h {remaining.Minutes}m";
            if (remaining.TotalMinutes >= 1)
                return $"{(int)remaining.TotalMinutes}m {remaining.Seconds}s";
            return $"{(int)remaining.TotalSeconds}s";
        }
    }
}
