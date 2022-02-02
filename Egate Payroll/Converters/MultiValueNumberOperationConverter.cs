using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Egate_Payroll.Converters
{
    public enum NumberOperation
    {
        Add, Subtract, Multiply, Divide
    }

    public class MultiValueNumberOperationConverter : IMultiValueConverter
    {
        public NumberOperation Operation { get; set; } = NumberOperation.Add;

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            decimal? result = null;
            if (values.Length > 0)
            {
                result = (decimal?)values[0];
                
                foreach (var val in values.Skip(1))
                {
                    if (result == null && val != null)
                    {
                        result = (decimal)val;
                        continue;
                    }
                    if (result != null && val != null)
                    {
                        switch (Operation)
                        {
                            case NumberOperation.Add:
                                result += (decimal)val;
                                break;
                            case NumberOperation.Subtract:
                                result -= (decimal)val;
                                break;
                            case NumberOperation.Multiply:
                                result *= (decimal)val;
                                break;
                            case NumberOperation.Divide:
                                result /= (decimal)val;
                                break;
                        }
                    }
                }
            }
            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
