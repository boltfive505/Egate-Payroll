using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Egate_Payroll.Objects
{
    public class EmployeeViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public int EmployeeId { get; set; } //database reference
        public int EmployeeNumber { get; set; }
        public string EmployeeName { get; set; }
        public string Department { get; set; }
        public EmployeeType EmployeeType { get; set; }
        public DateTime DateHired { get; set; }

        public decimal? MonthlyRate { get; set; }
        public decimal? HourlyRate { get; set; }
        public decimal? MealAllowance { get; set; }
        public decimal? TransportationAllowance { get; set; }
        public decimal? OtherAllowance { get; set; }
        public bool HasDeductions { get; set; }
        public string Notes { get; set; }

        public decimal YearToDateGrossIncome { get; set; }
        public decimal YearToDateAllowanceIncome { get; set; }
        public decimal YearToDateTotalIncome { get { return YearToDateGrossIncome + YearToDateAllowanceIncome; } }

        public bool IsActive { get; set; } = true;
    }
}
