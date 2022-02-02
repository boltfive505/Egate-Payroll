namespace Egate_Payroll.Deductions.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("pagibig")]
    public partial class pagibig
    {
        public long Id { get; set; }

        public decimal? MonthlyCompensationFrom { get; set; }

        public decimal? MonthlyCompensationTo { get; set; }

        public decimal EmployeeShareRate { get; set; }

        public decimal EmployerShareRate { get; set; }
    }
}
