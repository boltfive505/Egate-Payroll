using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Egate_Payroll.Model
{
    [Table("holiday")]
    public partial class holiday
    {
        public long Id { get; set; }

        [Required]
        public string HolidayType { get; set; }

        public long Date { get; set; }

        public string OtherName { get; set; }

        public decimal? OtherRate { get; set; }
    }
}
