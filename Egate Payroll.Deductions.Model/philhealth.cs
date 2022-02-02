namespace Egate_Payroll.Deductions.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("philhealth")]
    public partial class philhealth
    {
        public long Id { get; set; }

        public decimal MonthlyBasicSalaryFrom { get; set; }

        public decimal MonthlyBasicSalaryTo { get; set; }

        public decimal PremiumRate { get; set; }
    }
}
