using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Egate_Payroll.Model
{
    public partial class PayrollModel
    {
        public void Initialize()
        {
            Database.SetInitializer<PayrollModel>(null);
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
            Configuration.AutoDetectChangesEnabled = false;
            Database.Initialize(false);
            this.attendance.Load();
            Configuration.AutoDetectChangesEnabled = true;
        }
    }
}
