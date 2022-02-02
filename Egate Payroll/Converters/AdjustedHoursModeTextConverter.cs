
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Egate_Payroll.Converters
{
    [ValueConversion(typeof(AdjustedHoursMode), typeof(string))]
    public class AdjustedHoursModeTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "";
            var mode = (AdjustedHoursMode)value;
            switch (mode)
            {
                case AdjustedHoursMode.Add: return "+";
                case AdjustedHoursMode.Deduct: return "-";
                default: return Binding.DoNothing;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
