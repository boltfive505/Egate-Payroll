using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Egate_Payroll.Deductions.Model
{
    public partial class PayrollDeductionsModel
    {
        public void Initialize()
        {
            Database.SetInitializer<PayrollDeductionsModel>(null);
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
            Configuration.AutoDetectChangesEnabled = false;
            Database.Initialize(false);
            this.sss.Load();
            Configuration.AutoDetectChangesEnabled = true;
        }
    }
}
