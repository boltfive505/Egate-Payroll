using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyChanged;
using Egate_Payroll.Classes;

namespace Egate_Payroll.Objects
{
    public class EmployeeComputedPayrollViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public int AttendanceId { get; set; } //reference to database
        public int EmployeeId { get; set; } //database reference
        public int EmployeeNumber { get; set; }
        public string EmployeeName { get; set; }
        public EmployeeType EmployeeType { get; set; }
        public string Department { get; set; }
        public decimal? MonthlyRate { get; set; }
        public decimal? HourlyRate { get; set; }
        public decimal? TotalAllowance { get; set; }
        public bool HasDeductions { get; set; }
        public string Notes { get; set; }

        public DateTime WorkDate { get; set; }
        [AlsoNotifyFor("IsAbsent")]
        public DateTime? TimeIn { get; set; }
        [AlsoNotifyFor("IsAbsent")]
        public DateTime? TimeOut { get; set; }
        public bool IsAbsent { get { return TimeIn == null || TimeOut == null; } }
        public DateTime? ModifyTimeDate { get; set; }

        //holiday
        public HolidayType? HolidayType { get; set; }
        public string HolidayName { get; set; }
        public decimal? OtherHolidayRate { get; set; }
        public string HolidayFullName { get { return HolidayType == null ? null : string.Format("{0} - {1}", TypeDescriptor.GetConverter(HolidayType.Value).ConvertTo(HolidayType.Value, typeof(string)), HolidayName); } }
        public bool RegularHolidayAllowOvertime { get; set; }
        public bool SpecialHolidayAllowOvertime { get; set; }

        public bool IsMissingBothTime { get { return TimeIn == null && TimeOut == null; } }
        public bool IsMissingOneTime { get { return (TimeIn == null && TimeOut == null) == false && (TimeIn == null || TimeOut == null) == true; } }

        public TimeSpan? Late { get; set; }
        public TimeSpan? Early { get; set; }
        public TimeSpan? WorkTime { get; set; }
        public TimeSpan? Overtime { get; set; }

        //adjustment time
        public TimeSpan? AdjustmentTime { get; set; }
        public AdjustedHoursMode AdjustmentTimeMode { get; set; }
        public string AdjustmentTimeNotes { get; set; }

        //computed values
        public DateTime? ComputedTimeIn { get; set; }
        public DateTime? ComputedTimeOut { get; set; }
        public TimeSpan? ComputedLate { get; set; }
        public TimeSpan? ComputedEarly { get; set; }
        [AlsoNotifyFor("TotalWorkTime")]
        public TimeSpan? ComputedWorkTime { get; set; }
        [AlsoNotifyFor("TotalWorkTime")]
        public TimeSpan? ComputedOvertime { get; set; }

        public TimeSpan? ActualWorkTime
        {
            get
            {
                return ComputedWorkTime;
            }
        }

        public TimeSpan? ActualWorkOvertime
        {
            get
            {
                //if work day is holiday, and is not allowed to have overtime
                if (HolidayType != null && !IsAllowOvertimeForHoliday)
                {
                    return null;
                }
                return ComputedOvertime;
            }
        }

        public TimeSpan? TotalWorkTime
        {
            get
            {
                if (ActualWorkTime == null) return null;
                var overtimeHours = ActualWorkOvertime == null ? 0 : ActualWorkOvertime.Value.TotalHours;
                TimeSpan totalTime = TimeSpan.FromHours(ActualWorkTime.Value.TotalHours + overtimeHours + GetAdjustmentHours());
                return totalTime;
            }
        }

        public TimeSpan? HolidayRegularTime
        {
            get
            {
                //regular holiday if absent -> put 8 hours
                if (IsAbsent && IsValidForHolidayRate)
                {
                    if (HolidayType != null && HolidayType == Egate_Payroll.HolidayType.Regular)
                        return TimeSpan.FromHours(8);
                    else
                        return null;
                }
                else
                {
                    if (ComputedWorkTime == null || !IsValidForHolidayRate) 
                        return null;
                    else
                        return IncomeComputationHelper.GetHolidayPart(ComputedWorkTime.Value, HolidayType.Value, IsAllowOvertimeForHoliday && ComputedOvertime != null).RemoveSeconds();
                }
            }
        }

        public TimeSpan? HolidayOvertime
        {
            get
            {
                if (ComputedOvertime == null || !IsValidForHolidayRate || !IsAllowOvertimeForHoliday) return null;
                return IncomeComputationHelper.GetHolidayPart(ComputedOvertime.Value, HolidayType.Value, ComputedOvertime != null).RemoveSeconds();
            }
        }

        public TimeSpan? HolidayTotalTime
        {
            get
            {
                if (HolidayRegularTime == null || !IsValidForHolidayRate) return null;
                var totalHours = HolidayRegularTime.Value.TotalHours + (HolidayOvertime == null ? 0 : HolidayOvertime.Value.TotalHours);
                return TimeSpan.FromHours(totalHours);
            }
        }
        public TimeSpan? TotalTime
        {
            get
            {
                double total = (TotalWorkTime == null ? 0 : TotalWorkTime.Value.TotalHours) + (HolidayTotalTime == null ? 0 : HolidayTotalTime.Value.TotalHours);
                if (total == 0) return null;
                return TimeSpan.FromHours(total);
            }
        }

        public bool IsValidForHolidayRate
        {
            get { return HolidayType != null && EmployeeType == EmployeeType.Regular; }
        }

        public bool IsAllowOvertimeForHoliday
        {
            get
            {
                if (HolidayType == null) return false;
                switch (HolidayType.Value)
                {
                    case Egate_Payroll.HolidayType.Regular: return RegularHolidayAllowOvertime;
                    case Egate_Payroll.HolidayType.SpecialNonWorking: return SpecialHolidayAllowOvertime;
                    default: return false;
                }
            }
        }

        public double GetAdjustmentHours()
        {
            return IsAbsent || AdjustmentTime == null ? 0 : AdjustmentTime.Value.TotalHours * GetAdjustmentModeMultiplier();
        }

        private int GetAdjustmentModeMultiplier()
        {
            switch (AdjustmentTimeMode)
            {
                case AdjustedHoursMode.Add: return 1;
                case AdjustedHoursMode.Deduct: return -1;
                default: return 0;
            }
        }

        public void SetComputation1(ShiftSettingsViewModel settings)
        {
            ClearComputation();
            if (IsAbsent) //clear and skip computation if absent
                return;

            //only get time part of on/off duty
            DateTime start = new DateTime(WorkDate.Year, WorkDate.Month, WorkDate.Day, settings.OnDuty.Hour, settings.OnDuty.Minute, 0);
            DateTime end = new DateTime(WorkDate.Year, WorkDate.Month, WorkDate.Day, settings.OffDuty.Hour, settings.OffDuty.Minute, 0);
            DateTime inTime = new DateTime(WorkDate.Year, WorkDate.Month, WorkDate.Day, TimeIn.Value.Hour, TimeIn.Value.Minute, TimeIn.Value.Second);
            DateTime outTime = new DateTime(WorkDate.Year, WorkDate.Month, WorkDate.Day, TimeOut.Value.Hour, TimeOut.Value.Minute, TimeOut.Value.Second);

            if (inTime > start) //compute for late
                ComputedLate = (inTime - start);
            if (end > outTime) //compute for early
                ComputedEarly = (end - outTime);

            //compute for overtime
            TimeSpan? tempOvertime = null;
            if (outTime > end)
                tempOvertime = (outTime - end);
            if (tempOvertime != null && tempOvertime >= TimeSpan.FromMinutes(settings.OvertimeOffsetMinutes)) //if overtime is counted
                ComputedOvertime = tempOvertime;
            //deduct overtime offset, if enabled
            if (ComputedOvertime != null && settings.DeductOvertimeOffset)
                ComputedOvertime -= TimeSpan.FromMinutes(settings.OvertimeOffsetMinutes);

            //compute for worktime
            ComputedTimeIn = DateTimeExt.MinDateTime(start, (DateTime)inTime);
            ComputedTimeOut = DateTimeExt.MaxDateTime(end, (DateTime)outTime);
            ComputedWorkTime = ComputedTimeOut - ComputedTimeIn;
            //deduct computed worktime
            ComputedWorkTime -= TimeSpan.FromHours(1); //deduct from 1-hour lunch break
            //remove seconds part
            ComputedTimeIn = ComputedTimeIn.RemoveSeconds();
            ComputedTimeOut = ComputedTimeOut.RemoveSeconds();
            ComputedWorkTime = ComputedWorkTime.RemoveSeconds();
            ComputedOvertime = ComputedOvertime.RemoveSeconds();
            ComputedLate = ComputedLate.RemoveSeconds();
            ComputedEarly = ComputedEarly.RemoveSeconds();
        }

        public void ClearComputation()
        {
            ComputedTimeIn = null;
            ComputedTimeOut = null;
            ComputedLate = null;
            ComputedEarly = null;
            ComputedWorkTime = null;
            ComputedOvertime = null;
        }
    }
}
