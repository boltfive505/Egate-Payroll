using System;
using System.ComponentModel;

namespace Egate_Payroll
{
    [TypeConverter(typeof(Classes.EnumDescriptionTypeConverter))]
    public enum EmployeeType
    {
        Regular, 
        [Description("Non-Regular")]
        NonRegular
    }
}
