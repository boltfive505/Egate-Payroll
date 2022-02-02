using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CustomMonthlyCalendar
{
    public class DayClickEventArgs : RoutedEventArgs
    {
        public DateTime Day { get; set; }

        public DayClickEventArgs(DateTime day)
        {
            this.Day = day;
        }

        public DayClickEventArgs(RoutedEvent routedEvent, DateTime day) : base(routedEvent)
        {
            this.Day = day;
        }

        public DayClickEventArgs(RoutedEvent routedEvent, object source, DateTime day) : base(routedEvent, source)
        {
            this.Day = day;
        }
    }

    public delegate void DayClickEventHandler(object sender, DayClickEventArgs e);
}
