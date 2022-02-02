using System;
using System.ComponentModel;

namespace Egate_Payroll
{
    [TypeConverter(typeof(Classes.EnumDescriptionTypeConverter))]
    public enum HolidayType
    {
        [Description("Regular Holiday")]
        Regular,
        [Description("Special Non-Working Holiday")]
        SpecialNonWorking
    }
}
