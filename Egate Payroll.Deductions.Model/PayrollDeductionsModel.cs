using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;

namespace Egate_Payroll.Deductions.Model
{
    [DbConfigurationType(typeof(System.Data.SQLite.EF6.Configuration.SqliteDbConfiguration))]
    public partial class PayrollDeductionsModel : DbContext
    {
        public PayrollDeductionsModel()
            : base("data source=data/deductions.db;fail if missing=True;foreign keys=True;journal mode=Off")
        {
        }

        public virtual DbSet<pagibig> pagibig { get; set; }
        public virtual DbSet<philhealth> philhealth { get; set; }
        public virtual DbSet<sss> sss { get; set; }
        public virtual DbSet<tax> tax { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<DecimalPropertyConvention>();
            modelBuilder.Conventions.Add(new DecimalPropertyConvention(10, 2));
        }
    }
}
