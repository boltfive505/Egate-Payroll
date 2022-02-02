namespace Egate_Payroll.Tax_Calendar.Model
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class filing_form
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public filing_form()
        {
            period_inclusion = new ObservableCollection<period_inclusion>();
            filing_history = new ObservableCollection<filing_history>();
        }

        public long Id { get; set; }

        [Required]
        [StringLength(2147483647)]
        public string FormName { get; set; }

        [StringLength(2147483647)]
        public string FormTitle { get; set; }

        public long? FilingCategoryId { get; set; }

        [StringLength(2147483647)]
        public string Description { get; set; }

        [Required]
        [StringLength(2147483647)]
        public string PeriodType { get; set; }

        public long? DueDateStart { get; set; }

        public long? DueDateEnd { get; set; }

        public long? DueMonth { get; set; }

        public long? DueDays { get; set; }

        public long IsActive { get; set; }

        public virtual filing_category filing_category { get; set; }

        public virtual ObservableCollection<period_inclusion> period_inclusion { get; set; }

        public virtual ObservableCollection<filing_history> filing_history { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is filing_form)
            {
                filing_form o = (filing_form)obj;
                return this.Id == o.Id;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
