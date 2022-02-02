using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Egate_Payroll.Tax_Calendar.Model;

namespace Egate_Payroll.Objects.TaxCalendar
{
    public class TaxFilingCompanyViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public long Id { get; set; }
        public string CompanyName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; } = true;

        public TaxFilingCompanyViewModel()
        { }
            
        public TaxFilingCompanyViewModel(filing_company entity)
        {
            this.Id = entity.Id;
            this.CompanyName = entity.CompanyName;
            this.Description = entity.Description;
            this.IsActive = entity.IsActive.ToBool();
        }

        public override bool Equals(object obj)
        {
            if (obj is TaxFilingCompanyViewModel)
            {
                var o = obj as TaxFilingCompanyViewModel;
                return this.Id == o.Id;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
    }
}
