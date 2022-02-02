using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Egate_Payroll.Objects
{
    public class ShiftSettingsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public int ShiftSettingsId { get; set; } //database reference
        public DateTime OnDuty { get; set; }
        public DateTime OffDuty { get; set; }
        public int OvertimeOffsetMinutes { get; set; }
        public bool DeductOvertimeOffset { get; set; }

        public ShiftSettingsViewModel()
        {
            OnDuty = new DateTime(1, 1, 1, 8, 0, 0);
            OffDuty = new DateTime(1, 1, 1, 17, 0, 0);
            OvertimeOffsetMinutes = 10;
        }
    }
}
