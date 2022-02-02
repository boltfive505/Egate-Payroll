using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Egate_Payroll.Objects.TaxCalendar
{
    public class TaxFilingPaymentViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public int Id { get; set; } = -1; //database reference
        public TaxFilingPeriodViewModel Period { get; set; }
        public string FormName { get; set; }
        public string CategoryName { get; set; }
        public DateTime PeriodDate { get; set; }
        public DateTime FilingDate { get; set; }
        public decimal? Amount { get; set; }
        public TaxFilingCompanyViewModel Company { get; set; }
        public string FormFileName { get; set; }
        public string PaymentFileName { get; set; }
        public string ProvisionFileName { get; set; }
        public string Notes { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is TaxFilingPaymentViewModel)
            {
                var o = obj as TaxFilingPaymentViewModel;
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
