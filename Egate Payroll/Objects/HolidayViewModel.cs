using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Egate_Payroll.Objects
{
    public class HolidayViewModel : CustomMonthlyCalendar.IMonthlyCalendarDayItem, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public int Id { get; set; }
        public HolidayType HolidayType { get; set; }
        public DateTime Day { get; set; }
        public string OtherName { get; set; }
        public decimal? OtherRate { get; set; }
    }
}
