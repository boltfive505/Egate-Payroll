namespace Egate_Payroll.Tax_Calendar.Model
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class filing_history
    {
        public long Id { get; set; }

        public long PeriodId { get; set; }

        public string FormName { get; set; }

        public long PeriodDate { get; set; }

        public long FilingDate { get; set; }

        public decimal Amount { get; set; }

        public long? CompanyId { get; set; }

        public string FormFileAttachment { get; set; }

        public string PaymentFileAttachment { get; set; }

        public string ProvisionFileAttachment { get; set; }

        public string Notes { get; set; }

        public long? UpdatedDate { get; set; }

        public virtual filing_form filing_form { get; set; }

        public virtual filing_company filing_company { get; set; }
    }
}
