namespace Egate_Payroll.Tax_Calendar.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class period_inclusion
    {
        public long Id { get; set; }

        public long PeriodId { get; set; }

        [Required]
        public string InclusionType { get; set; }

        public long Value { get; set; }

        public long IsIncluded { get; set; }

        public virtual filing_form filing_form { get; set; }
    }
}
