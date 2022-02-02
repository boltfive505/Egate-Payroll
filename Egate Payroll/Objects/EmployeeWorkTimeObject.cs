using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomMappingObject;

namespace Egate_Payroll.Objects
{
    public class EmployeeWorkTimeObject
    {
        [ColumnMapping("User No.")]
        public int EmployeeNumber { get; set; }

        [ColumnMapping("Name")]
        public string EmployeeName { get; set; }

        [ColumnMapping("Department"), SkipMappingIfMissing]
        public string Department { get; set; }

        [ColumnMapping("Date")]
        public DateTime WorkDate { get; set; }

        [ColumnMapping("On Duty"), SkipMappingIfMissing]
        public DateTime? OnDuty { get; set; }

        [ColumnMapping("Off Duty"), SkipMappingIfMissing]
        public DateTime? OffDuty { get; set; }

        [ColumnMapping("In")]
        public DateTime? TimeIn { get; set; }

        [ColumnMapping("Out")]
        public DateTime? TimeOut { get; set; }

        [ColumnMapping("Late"), SkipMappingIfMissing]
        public TimeSpan? Late { get; set; }

        [ColumnMapping("Early"), SkipMappingIfMissing]
        public TimeSpan? Early { get; set; }

        [ColumnMapping("OverTime")]
        public TimeSpan? Overtime { get; set; }

        [ColumnMapping("Actual Time")]
        public TimeSpan? WorkTime { get; set; }

        [ColumnMapping("Absent"), SkipMappingIfMissing]
        public bool IsAbsent { get; set; }

        [IgnoreMapping]
        public int CutoffId { get; set; }
    }
}
