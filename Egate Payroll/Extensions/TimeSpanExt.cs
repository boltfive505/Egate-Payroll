using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Egate_Payroll
{
    public static class TimeSpanExt
    {
        public static decimal ToTotalHours(this TimeSpan value)
        {
            return (decimal)Math.Round(value.TotalHours, 2);
        }

        public static decimal? ToTotalHours(this TimeSpan? value)
        {
            if (value == null) return null;
            return (decimal?)ToTotalHours((TimeSpan)value);
        }

        public static TimeSpan ToTimeSpan(this decimal value)
        {
            return TimeSpan.FromHours((double)value);
        }

        public static TimeSpan? ToTimeSpan(this decimal? value)
        {
            if (value == null || value == 0) return null;
            return (TimeSpan?)ToTimeSpan((decimal)value);
        }

        public static TimeSpan Multiply(this TimeSpan value, double multiplier)
        {
            return TimeSpan.FromTicks((long)(value.Ticks * multiplier));
        }

        public static TimeSpan? Multiply(this TimeSpan? value, double multiplier)
        {
            if (value == null) return null;
            return (TimeSpan?)TimeSpan.FromTicks((long)(value.Value.Ticks * multiplier));
        }

        public static TimeSpan RemoveSeconds(this TimeSpan value)
        {
            return TimeSpan.FromMinutes((int)value.TotalMinutes);
        }

        public static TimeSpan? RemoveSeconds(this TimeSpan? value)
        {
            if (value == null) return null;
            return RemoveSeconds(value.Value);
        }
    }
}
