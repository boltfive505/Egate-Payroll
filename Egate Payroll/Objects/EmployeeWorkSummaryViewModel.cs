using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Egate_Payroll.Objects
{
    public class EmployeeWorkSummaryViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public int EmployeeId { get; set; } //database reference
        public int EmployeeNumber { get; set; }
        public string EmployeeName { get; set; }
        public EmployeeType EmployeeType { get; set; }
        public string Department { get; set; }
        public decimal MonthlyRate { get; set; }
        public decimal HourlyRate { get; set; }
        public decimal AllowanceRate { get; set; }
        public bool HasDeductions { get; set; }
        public string Notes { get; set; }

        public decimal AdjustmentAmount { get; set; }

        public TimeSpan SubtotalWorkTime { get; set; }
        public TimeSpan SubtotalOvertime { get; set; }
        public TimeSpan TotalWorkTime { get; set; }

        public TimeSpan SubtotalHolidayTime { get; set; }
        public TimeSpan SubtotalHolidayOvertime { get; set; }
        public TimeSpan TotalHolidayTime { get; set; }

        public TimeSpan TotalAdjustmentTime { get; set; }
        public AdjustedHoursMode FinalAdjustmentHoursMode { get; set; }

        public TimeSpan? FinalRegularHours { get; set; }
        public TimeSpan? FinalOvertime { get; set; }

        public TimeSpan TotalHours 
        {
            get 
            {
                double regularHours = (FinalRegularHours == null ? 0 : FinalRegularHours.Value.TotalHours) + (FinalOvertime == null ? 0 : FinalOvertime.Value.TotalHours * 1.25);
                double holidayHours = TotalHolidayTime.TotalHours;
                return TimeSpan.FromHours(regularHours + holidayHours); 
            } 
        }

        public decimal FinalRegularHoursIncome
        {
            get { return Math.Round((decimal)(FinalRegularHours?.TotalHours ?? 0) * HourlyRate, 2); }
        }

        public decimal FinalOvertimeIncome
        {
            get { return Math.Round((decimal)(FinalOvertime?.TotalHours ?? 0) * HourlyRate, 2); }
        }

        public decimal TotalHolidayIncome
        {
            get { return Math.Round((decimal)TotalHolidayTime.TotalHours * HourlyRate, 2); }
        }

        public decimal GrossIncome { get { return Math.Round((decimal)TotalHours.TotalHours * HourlyRate, 2); } }

        public decimal GrandTotalAllowance { get { return Math.Round((decimal)(FinalRegularHours == null ? 0 : FinalRegularHours.Value.TotalHours) * AllowanceRate, 2); } }

        public decimal NetPay
        {
            get
            {
                return Math.Round((Math.Max(GrossIncome, 0) - (Deductions.TotalEmployeeDeductions)) + GrandTotalAllowance + AdjustmentAmount, 2);
            }
        }

        public PayrollDeductionsViewModel Deductions { get; set; }

        public EmployeeWorkSummaryViewModel()
        {
            Deductions = new PayrollDeductionsViewModel();
        }
    }
}
