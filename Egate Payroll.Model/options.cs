namespace Egate_Payroll.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("options")]
    public partial class options
    {
        public long Id { get; set; }

        public long RegularHolidayAllowOvertime { get; set; }

        public long SpecialHolidayAllowOvertime { get; set; }
    }
}
