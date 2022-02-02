namespace Egate_Payroll.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("attendance_summary")]
    public partial class attendance_summary
    {
        public long Id { get; set; }

        public long EmployeeId { get; set; }

        public long CutoffId { get; set; }

        public decimal? AdjustmentAmount { get; set; }

        public decimal? FinalRegularHours { get; set; }

        public decimal? FinalOvertimeHours { get; set; }

        public decimal? TotalHolidayHours { get; set; }

        public decimal? TotalHours { get; set; }

        public decimal? GrossIncome { get; set; }

        public decimal? AllowanceIncome { get; set; }

        public decimal? SssDeductionTotal { get; set; }

        public decimal? PhilhealthDeductionTotal { get; set; }

        public decimal? PagibigDeductionTotal { get; set; }

        public decimal? TaxDeductionTotal { get; set; }

        public decimal? NetPay { get; set; }

        public virtual employee employee { get; set; }

        public virtual cutoff cutoff { get; set; }
    }
}
