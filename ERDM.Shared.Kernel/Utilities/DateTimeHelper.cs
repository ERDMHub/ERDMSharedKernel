using System;

namespace ERDM.Shared.Kernel.Utilities
{
    public static class DateTimeHelper
    {
        public static DateTimeOffset Now => DateTimeOffset.UtcNow;

        public static DateTimeOffset StartOfDay(this DateTimeOffset dateTime)
        {
            return new DateTimeOffset(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, dateTime.Offset);
        }

        public static DateTimeOffset EndOfDay(this DateTimeOffset dateTime)
        {
            return new DateTimeOffset(dateTime.Year, dateTime.Month, dateTime.Day, 23, 59, 59, dateTime.Offset);
        }

        public static DateTimeOffset StartOfMonth(this DateTimeOffset dateTime)
        {
            return new DateTimeOffset(dateTime.Year, dateTime.Month, 1, 0, 0, 0, dateTime.Offset);
        }

        public static DateTimeOffset EndOfMonth(this DateTimeOffset dateTime)
        {
            var lastDay = DateTime.DaysInMonth(dateTime.Year, dateTime.Month);
            return new DateTimeOffset(dateTime.Year, dateTime.Month, lastDay, 23, 59, 59, dateTime.Offset);
        }

        public static DateTimeOffset StartOfYear(this DateTimeOffset dateTime)
        {
            return new DateTimeOffset(dateTime.Year, 1, 1, 0, 0, 0, dateTime.Offset);
        }

        public static DateTimeOffset EndOfYear(this DateTimeOffset dateTime)
        {
            return new DateTimeOffset(dateTime.Year, 12, 31, 23, 59, 59, dateTime.Offset);
        }

        public static string ToIsoString(this DateTimeOffset dateTime)
        {
            return dateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        }

        public static DateTimeOffset? FromIsoString(string isoString)
        {
            if (DateTimeOffset.TryParse(isoString, out var result))
                return result;
            return null;
        }
    }
}