using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Egate_Payroll.Converters
{
    [ValueConversion(typeof(decimal?), typeof(decimal?))]
    public class PercentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var num = (decimal?)value;
            if (num == null) return null;
            else return num * 100;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var num = (decimal?)value;
            if (num == null) return null;
            else return num / 100;
        }
    }
}
