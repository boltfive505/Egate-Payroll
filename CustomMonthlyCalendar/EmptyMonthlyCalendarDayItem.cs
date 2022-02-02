using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomMonthlyCalendar
{
    public class EmptyMonthlyCalendarDayItem : IMonthlyCalendarDayItem
    {
        public DateTime Day { get; set; }
    }
}
