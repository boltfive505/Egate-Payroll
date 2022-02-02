namespace Egate_Payroll.Tax_Calendar.Model
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("filing_company")]
    public partial class filing_company
    {
        public filing_company()
        {
            filing_histories = new HashSet<filing_history>();
        }

        public long Id { get; set; }

        public string CompanyName { get; set; }

        public string Description { get; set; }

        public long IsActive { get; set; } = 1;

        public ICollection<filing_history> filing_histories { get; set; }
    }
}
