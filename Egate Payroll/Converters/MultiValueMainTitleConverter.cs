using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Egate_Payroll.Objects;

namespace Egate_Payroll.Converters
{
    public class MultiValueMainTitleConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string title = (string)values[0];
            CutoffViewModel cutoff = null;
            if (values[1] is CutoffViewModel)
                cutoff = values[1] as CutoffViewModel;

            return string.Format("{0}   [{1}]", 
                title,
                cutoff == null ? "Please select Cover Period" : string.Format("Cover Period: {0} - {1}", cutoff.StartDate.ToString("yyyy-MM-dd"), cutoff.EndDate.ToString("yyyy-MM-dd")));
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
