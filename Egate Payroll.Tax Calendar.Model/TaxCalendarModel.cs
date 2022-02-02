using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace Egate_Payroll.Tax_Calendar.Model
{
    [DbConfigurationType(typeof(System.Data.SQLite.EF6.Configuration.SqliteDbConfiguration))]
    public partial class TaxCalendarModel : DbContext
    {
        public TaxCalendarModel()
            : base("data source=data/tax calendar.db;fail if missing=True;foreign keys=True;journal mode=off")
        {
        }

        public virtual DbSet<filing_form> filing_form { get; set; }
        public virtual DbSet<filing_history> filing_history { get; set; }
        public virtual DbSet<filing_category> filing_category { get; set; }
        public virtual DbSet<filing_company> filing_company { get; set; }
        public virtual DbSet<period_inclusion> period_inclusion { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<filing_form>()
                .HasOptional(e => e.filing_category)
                .WithMany(e => e.filing_form)
                .HasForeignKey(e => e.FilingCategoryId);

            modelBuilder.Entity<period_inclusion>()
                .HasRequired(e => e.filing_form)
                .WithMany(e => e.period_inclusion)
                .HasForeignKey(e => e.PeriodId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<filing_history>()
                .HasRequired(e => e.filing_form)
                .WithMany(e => e.filing_history)
                .HasForeignKey(e => e.PeriodId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<filing_history>()
                .HasOptional(e => e.filing_company)
                .WithMany(e => e.filing_histories)
                .HasForeignKey(e => e.CompanyId)
                .WillCascadeOnDelete(false);
        }
    }
}
