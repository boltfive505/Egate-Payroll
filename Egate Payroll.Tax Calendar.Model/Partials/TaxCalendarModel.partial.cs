using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Egate_Payroll.Tax_Calendar.Model
{
    public partial class TaxCalendarModel
    {
        public void Initialize()
        {
            Database.SetInitializer<TaxCalendarModel>(null);
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
            Configuration.AutoDetectChangesEnabled = false;
            Database.Initialize(false);
            this.filing_form.Load();
            Configuration.AutoDetectChangesEnabled = true;
        }
    }
}
