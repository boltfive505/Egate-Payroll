using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Egate_Payroll.Objects.TaxCalendar
{
    public class TaxFilingPeriodInclusionViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public int Id { get; set; } = -1; //database reference

        public int Value { get; set; }

        public bool IsIncluded { get; set; } = true;
    }
}
