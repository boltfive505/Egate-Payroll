namespace Egate_Payroll.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("attendance")]
    public partial class attendance
    {
        public long Id { get; set; }

        public long EmployeeId { get; set; }

        public long CutoffId { get; set; }

        public long WorkDate { get; set; }

        public long? OnDuty { get; set; }

        public long? OffDuty { get; set; }

        public long? TimeIn { get; set; }

        public long? TimeOut { get; set; }

        public decimal? LateHours { get; set; }

        public decimal? EarlyHours { get; set; }

        public decimal? OvertimeHours { get; set; }

        public decimal? WorkTimeHours { get; set; }

        public long IsAbsent { get; set; }

        public long? ComputedTimeIn { get; set; }

        public long? ComputedTimeOut { get; set; }

        public decimal? ComputedLateHours { get; set; }

        public decimal? ComputedEarlyHours { get; set; }

        public decimal? ComputedOvertimeHours { get; set; }

        public decimal? ComputedWorkTimeHours { get; set; }

        public long? ModifyTimeDate { get; set; }

        public decimal? AdjustedHours { get; set; }

        public string AdjustedHoursMode { get; set; }

        public string AdjustedHoursNotes { get; set; }

        public virtual employee employee { get; set; }

        public virtual cutoff cutoff { get; set; }
    }
}
