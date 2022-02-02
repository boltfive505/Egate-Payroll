using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Egate_Payroll.Classes
{
    public static class IncomeComputationHelper
    {
        public static decimal? AllowanceComputation(TimeSpan? WorkTime, TimeSpan? Overtime, decimal? AllowanceRate)
        {
            if (WorkTime == null || AllowanceRate == null) return null;
            double totalTime = WorkTime.Value.TotalHours + (Overtime == null ? 0 : Overtime.Value.TotalHours);
            return (decimal)totalTime * AllowanceRate.Value;
        }

        public static TimeSpan GetHolidayPart(TimeSpan value, HolidayType holidayType, bool hasOvertime)
        {
            var multiplier = GetHolidayMultiplier(holidayType, hasOvertime);
            return TimeSpan.FromHours(value.TotalHours * multiplier);
        }

        private static double GetHolidayMultiplier(HolidayType holidayType, bool hasOvertime)
        {
            double multiplier = 0;
            switch (holidayType)
            {
                case HolidayType.Regular:
                    multiplier = 1;
                    break;
                case HolidayType.SpecialNonWorking:
                    multiplier = 0.3;
                    break;
            }
            if (hasOvertime)
                multiplier += 0.3;
            return multiplier;
        }
    }
}
