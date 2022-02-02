namespace Egate_Payroll.Deductions.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("sss")]
    public partial class sss
    {
        public long Id { get; set; }

        public decimal? CompensationFrom { get; set; }

        public decimal? CompensationTo { get; set; }

        public decimal EmployerContribution { get; set; }

        public decimal EmployeeContribution { get; set; }
    }
}
