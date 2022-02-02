using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Egate_Payroll.Converters
{
    [ValueConversion(typeof(TimeSpan?), typeof(string))]
    public class TimeSpanTotalHoursDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            TimeSpan? ts = (TimeSpan?)value;
            return string.Format("{0}:{1:00}:{2:00}", (int)ts.Value.TotalHours, ts.Value.Minutes, ts.Value.Seconds);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
