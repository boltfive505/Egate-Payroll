using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Egate_Payroll.Objects.TaxCalendar
{
    public class TaxFilingPeriodViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public int Id { get; set; } = -1; //database reference
        public string FormName { get; set; }
        public string FormTitle { get; set; }
        public TaxFilingCategoryViewModel Category { get; set; }
        public string Description { get; set; }
        public FilingPeriodType PeriodType { get; set; } = FilingPeriodType.Monthly;
        public DateTime? DueDateStart { get; set; }
        public DateTime? DueDateEnd { get; set; }
        public int? DueMonth { get; set; }
        public int? DueDays { get; set; }

        public string DueDateStr
        {
            get
            {
                switch (PeriodType)
                {
                    case FilingPeriodType.OneTime:
                        return DueDateStart?.ToString("MMM dd, yyyy");
                    case FilingPeriodType.Monthly:
                    case FilingPeriodType.EndOfQuarter:
                        return string.Format("{0} day", new Converters.OrdinalStringConverter().Convert(DueDays, null, null, null));
                    case FilingPeriodType.Annually:
                        return string.Format("{0} {1}", new Converters.MonthNameConverter().Convert(DueMonth, null, null, null), DueDays);
                    default:
                        return "";
                }
            }
        }

        public List<TaxFilingPeriodInclusionViewModel> MonthInclusionList { get; set; }

        public List<TaxFilingPeriodInclusionViewModel> QuarterInclusionList { get; set; }

        public bool IsActive { get; set; } = true;

        public bool HasExclusion
        {
            get
            {
                switch (PeriodType)
                {
                    case FilingPeriodType.Monthly:
                        return MonthInclusionList.Any(i => !i.IsIncluded);
                    case FilingPeriodType.EndOfQuarter:
                        return QuarterInclusionList.Any(i => !i.IsIncluded);
                    default:
                        return false;
                }
            }
        }

        public TaxFilingPeriodViewModel()
        {
            MonthInclusionList = Enumerable.Range(1, 12)
                .Select(n => new TaxFilingPeriodInclusionViewModel() {Value = n })
                .ToList();

            QuarterInclusionList = Enumerable.Range(1, 4)
                .Select(n => new TaxFilingPeriodInclusionViewModel() { Value = n })
                .ToList();
        }

        public IEnumerable<DateTime> GetPeriodDatesByYear(int year)
        {
            List<DateTime> dates = new List<DateTime>();
            try
            {
                switch (PeriodType)
                {
                    case FilingPeriodType.OneTime:
                        dates.Add(DueDateStart.Value);
                        break;
                    case FilingPeriodType.Monthly:
                        dates.AddRange(GetMonthlyDates(year, DueDays.Value));
                        break;
                    case FilingPeriodType.EndOfQuarter:
                        dates.AddRange(GetEndOfQuarterDates(year, DueDays.Value));
                        break;
                    case FilingPeriodType.Annually:
                        dates.Add(new DateTime(year, DueMonth.Value, DueDays.Value));
                        break;
                }
            }
            catch
            { }
            return dates;
        }

        private IEnumerable<DateTime> GetMonthlyDates(int year, int day)
        {
            return Enumerable.Range(1, 12)
                .Where(n => MonthInclusionList.First(i => i.Value == n).IsIncluded)
                .Select(n => new DateTime(year, n, day));
        }

        private IEnumerable<DateTime> GetEndOfQuarterDates(int year, int daysOffset)
        {
            List<DateTime> dates = new List<DateTime>();
            //include last quarter from last year
            if (QuarterInclusionList.First(i => i.Value == 4).IsIncluded)
            {
                dates.Add(new DateTime(year - 1, 12, DateTime.DaysInMonth(year - 1, 12)).AddDays(daysOffset));
            }

            //add from first 3 quarters of year
            //not include last quarter of this year
            dates.AddRange(Enumerable.Range(1, 3)
                .Where(n => QuarterInclusionList.First(i => i.Value == n).IsIncluded)
                .Select(n =>
                {
                    int month = n * 3;
                    return new DateTime(year, month, DateTime.DaysInMonth(year, month)).AddDays(daysOffset);
                }));
            return dates;
        }
    }
}
