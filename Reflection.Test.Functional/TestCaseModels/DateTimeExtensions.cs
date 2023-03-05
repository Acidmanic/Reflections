using System;

namespace Reflection.Test.Functional.TestCaseModels
{
    public static class DateTimeExtensions
    {
        public static long GetTimestamp(this DateTime dateTime)
        {
            return (long) dateTime.ToUniversalTime().Subtract(
                new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            ).TotalMilliseconds;
        }
        
        public static long GetTotalMilliseconds(this DateTime dateTime)
        {
            var baseDate = new DateTime(1970, 1, 1);

            var difference = dateTime.Subtract(baseDate).TotalMilliseconds;

            return (long) difference;
        }
    }
}