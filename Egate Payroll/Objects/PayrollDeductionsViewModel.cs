using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Egate_Payroll.Objects
{
    public class PayrollDeductionsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        //sss
        public decimal SssEmployeeContribution { get; set; }
        public decimal SssEmployerContribution { get; set; }
        //philhealth
        public decimal PhilhealthEmployeeContribution { get; set; }
        public decimal PhilhealthEmployerContribution { get; set; }
        //pagibig
        public decimal PagibigEmployeeContribution { get; set; }
        public decimal PagibigEmployerContribution { get; set; }
        //withholding tax
        public decimal TaxEmployeeContribution { get; set; }
        //public decimal TaxEmployerContribution { get; set; } //employer has no tax share

        public decimal TotalEmployeeDeductions
        {
            get { return SssEmployeeContribution + PhilhealthEmployeeContribution + PagibigEmployeeContribution + TaxEmployeeContribution; }
        }

        public decimal TotalEmployerDeductions
        {
            get { return SssEmployerContribution + PhilhealthEmployerContribution + PagibigEmployerContribution; }
        }
    }
}
