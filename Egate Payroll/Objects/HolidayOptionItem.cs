using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Egate_Payroll.Objects
{
    public class HolidayOptionItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public HolidayType HolidayType { get; set; }
        public string OtherName { get; set; }
        public decimal? OtherRate { get; set; } = 1;

        public override bool Equals(object obj)
        {
            if (obj is HolidayOptionItem)
            {
                HolidayOptionItem o = obj as HolidayOptionItem;
                return this.HolidayType == o.HolidayType;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HolidayType.GetHashCode();
        }
    }
}
