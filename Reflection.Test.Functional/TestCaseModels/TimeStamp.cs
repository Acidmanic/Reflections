using System;

namespace Reflection.Test.Functional.TestCaseModels
{

    public class TimeStamp
    {
        public long TotalMilliSeconds { get; }

        public TimeStamp(long totalMilliSeconds)
        {
            TotalMilliSeconds = totalMilliSeconds;
        }


        public static implicit operator TimeStamp(long timestamp)
        {
            return new TimeStamp(timestamp);
        }

        public static implicit operator long(TimeStamp timeStamp)
        {
            return timeStamp.TotalMilliSeconds;
        }

        public static implicit operator TimeStamp(string timestamp)
        {
            var tsLong = long.Parse(timestamp);

            return new TimeStamp(tsLong);
        }

        public static implicit operator string(TimeStamp timeStamp)
        {
            return timeStamp.TotalMilliSeconds.ToString();
        }

        public static implicit operator TimeStamp(DateTime timestamp)
        {
            return timestamp.GetTotalMilliseconds();
        }

        public static implicit operator DateTime(TimeStamp timeStamp)
        {
            var ticks = TimeSpan.TicksPerMillisecond * timeStamp.TotalMilliSeconds;

            var ts = new TimeSpan(ticks);

            var date = DateTime.UnixEpoch.Add(ts);

            return date;
        }

        public static implicit operator TimeStamp(TimeSpan timestamp)
        {
            return (long)timestamp.TotalMilliseconds;
        }

        public static implicit operator TimeSpan(TimeStamp timeStamp)
        {
            var ticks = TimeSpan.TicksPerMillisecond * timeStamp.TotalMilliSeconds;

            return new TimeSpan(ticks);
        }

        public static TimeStamp Now => DateTime.Now;


        public long CompareTo(TimeStamp value)
        {
            return TotalMilliSeconds - value.TotalMilliSeconds;
        }


        public long CompareTo(TimeSpan value)
        {
            return TotalMilliSeconds - (long)value.TotalMilliseconds;
        }

        public long CompareTo(DateTime value)
        {
            return TotalMilliSeconds - value.GetTotalMilliseconds();
        }
    }
}