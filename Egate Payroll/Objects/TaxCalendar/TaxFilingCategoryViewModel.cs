using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Egate_Payroll.Tax_Calendar.Model;

namespace Egate_Payroll.Objects.TaxCalendar
{
    public class TaxFilingCategoryViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public long Id { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }

        public TaxFilingCategoryViewModel()
        { }

        public TaxFilingCategoryViewModel(filing_category entity)
        {
            this.Id = entity.Id;
            this.CategoryName = entity.CategoryName;
            this.Description = entity.Description;
        }

        public override bool Equals(object obj)
        {
            if (obj is TaxFilingCategoryViewModel)
            {
                var o = obj as TaxFilingCategoryViewModel;
                return this.Id == o.Id;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public override string ToString()
        {
            return this.CategoryName;
        }
    }
}
