using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;

namespace Egate_Payroll.Model
{
    [DbConfigurationType(typeof(System.Data.SQLite.EF6.Configuration.SqliteDbConfiguration))]
    public partial class PayrollModel : DbContext
    {
        public PayrollModel()
            : base("data source=data/payroll.db;fail if missing=True;foreign keys=True;journal mode=Off")
        {
        }

        public virtual DbSet<attendance> attendance { get; set; }
        public virtual DbSet<attendance_summary> attendance_summary { get; set; }
        public virtual DbSet<cutoff> cutoff { get; set; }
        public virtual DbSet<shift_settings> shift_settings { get; set; }
        public virtual DbSet<employee> employee { get; set; }
        public virtual DbSet<holiday> holiday { get; set; }
        public virtual DbSet<options> options { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<DecimalPropertyConvention>();
            modelBuilder.Conventions.Add(new DecimalPropertyConvention(10, 2));

            modelBuilder.Entity<cutoff>()
                .HasMany(e => e.attendance)
                .WithRequired(e => e.cutoff)
                .HasForeignKey(e => e.CutoffId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<employee>()
                .HasMany(e => e.attendance)
                .WithRequired(e => e.employee)
                .HasForeignKey(e => e.EmployeeId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<attendance_summary>()
                .HasRequired(e => e.employee)
                .WithMany(e => e.attendance_summary)
                .HasForeignKey(e => e.EmployeeId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<attendance_summary>()
                .HasRequired(e => e.cutoff)
                .WithMany(e => e.attendance_summary)
                .HasForeignKey(e => e.CutoffId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<shift_settings>()
                .HasRequired(e => e.cutoff)
                .WithMany()
                .HasForeignKey(e => e.CutoffId)
                .WillCascadeOnDelete(true);
        }
    }
}
