namespace Egate_Payroll.Tax_Calendar.Model
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class filing_category
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public filing_category()
        {
            filing_form = new ObservableCollection<filing_form>();
        }

        public long Id { get; set; }

        [Required]
        [StringLength(2147483647)]
        public string CategoryName { get; set; }

        [StringLength(2147483647)]
        public string Description { get; set; }

        public virtual ObservableCollection<filing_form> filing_form { get; set; }
    }
}
