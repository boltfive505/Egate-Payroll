using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Egate_Payroll.Objects
{
    public class CutoffViewModel : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Days { get { return (int)(EndDate - StartDate).TotalDays; } }
        public decimal? RegularHours { get; set; }
        public DateTime ImportTime { get; set; }
        public int EmployeeCount { get; set; }
        public decimal TotalWorkTime { get; set; }

        public bool IsSelected { get; set; } //only for selecting in ui

        public event PropertyChangedEventHandler PropertyChanged;
        public override bool Equals(object obj)
        {
            if (obj is CutoffViewModel)
            {
                return this.Id == (obj as CutoffViewModel).Id;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
