using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Egate_Payroll.Converters
{

    [ValueConversion(typeof(decimal), typeof(decimal))]
    public class NumberOperationConverter : IValueConverter
    {
        public NumberOperation Operation { get; set; } = NumberOperation.Add;
        public decimal OperatorValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            decimal num = (decimal)value;
            switch (Operation)
            {
                case NumberOperation.Add:
                    return num + OperatorValue;
                case NumberOperation.Subtract:
                    return num - OperatorValue;
                case NumberOperation.Multiply:
                    return num *= OperatorValue;
                case NumberOperation.Divide:
                    return num /= OperatorValue;
            }
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
