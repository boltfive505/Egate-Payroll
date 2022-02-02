namespace Egate_Payroll.Model
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("cutoff")]
    public partial class cutoff
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public cutoff()
        {
            attendance = new ObservableCollection<attendance>();
            attendance_summary = new ObservableCollection<attendance_summary>();
        }

        public long Id { get; set; }

        public long StartDate { get; set; }

        public long EndDate { get; set; }

        public decimal? RegularHours { get; set; }

        public long ImportTime { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ObservableCollection<attendance> attendance { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ObservableCollection<attendance_summary> attendance_summary { get; set; }
    }
}
