using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Egate_Payroll
{
    [TypeConverter(typeof(Classes.EnumDescriptionTypeConverter))]
    public enum FilingPeriodType
    {
        [Description("Monthly")]
        Monthly,
        [Description("End of Quarter")]
        EndOfQuarter,
        [Description("Yearly")]
        Annually,
        [Description("One-Time")]
        OneTime
    }
}
