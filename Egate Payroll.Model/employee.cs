namespace Egate_Payroll.Model
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("employee")]
    public partial class employee
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public employee()
        {
            attendance = new ObservableCollection<attendance>();
            attendance_summary = new ObservableCollection<attendance_summary>();
        }

        public long Id { get; set; }

        public long EmployeeNumber { get; set; }

        [Required]
        [StringLength(2147483647)]
        public string EmployeeName { get; set; }

        [StringLength(2147483647)]
        public string Department { get; set; }

        public decimal? MonthlyRate { get; set; }

        public decimal? HourlyRate { get; set; }

        public decimal? MealAllowance { get; set; }

        public decimal? TransportationAllowance { get; set; }

        public decimal? OtherAllowance { get; set; }

        [Required]
        public string EmployeeType { get; set; }

        public long DateHired { get; set; }

        public long IsActive { get; set; }

        public long HasDeductions { get; set; }

        public string Notes { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ObservableCollection<attendance> attendance { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ObservableCollection<attendance_summary> attendance_summary { get; set; }
    }
}
