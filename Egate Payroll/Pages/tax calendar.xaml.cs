using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Egate_Payroll.Objects.TaxCalendar;
using Egate_Payroll.Classes;
using System.Data.Entity;
using System.ComponentModel;
using System.IO;
using CustomControls.Modal;

namespace Egate_Payroll.Pages
{
    /// <summary>
    /// Interaction logic for tax_calendar.xaml
    /// </summary>
    public partial class tax_calendar : Page
    {
        public class FilterGroup : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            public string FilterKeyword { get; set; }
            public int FilterCategory { get; set; }
            public bool FilterInactive { get; set; }

            public Dictionary<int, string> CategoryList { get; set; } = new Dictionary<int, string>();
        }

        public class PeriodCalendarDisplayCollection : INotifyPropertyChanged, CustomMonthlyCalendar.IMonthlyCalendarDayItem
        {
            public event PropertyChangedEventHandler PropertyChanged;

            public DateTime Day { get; set; }
            public IEnumerable<PeriodCalendarDisplay> Items { get; set; }
        }

        public class PeriodCalendarDisplay : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            public DateTime PeriodDate { get; set; }
            public TaxFilingPeriodViewModel Item { get; set; }
        }

        public static readonly DependencyProperty ItemFilingPeriodListProperty = DependencyProperty.Register(nameof(ItemFilingPeriodList), typeof(ICollectionView), typeof(tax_calendar));
        public ICollectionView ItemFilingPeriodList
        {
            get { return (ICollectionView)GetValue(ItemFilingPeriodListProperty); }
            set { SetValue(ItemFilingPeriodListProperty, value); }
        }

        public static readonly DependencyProperty FilingPeriodsDayListProperty = DependencyProperty.Register(nameof(FilingPeriodsDayList), typeof(ICollectionView), typeof(tax_calendar));
        public ICollectionView FilingPeriodsDayList
        {
            get { return (ICollectionView)GetValue(FilingPeriodsDayListProperty); }
            set { SetValue(FilingPeriodsDayListProperty, value); }
        }


        public static readonly DependencyProperty FiltersProperty = DependencyProperty.Register(nameof(Filters), typeof(FilterGroup), typeof(tax_calendar));
        public FilterGroup Filters
        {
            get { return (FilterGroup)GetValue(FiltersProperty); }
            set { SetValue(FiltersProperty, value); }
        }

        private List<TaxFilingPeriodViewModel> list = new List<TaxFilingPeriodViewModel>();
        private List<PeriodCalendarDisplayCollection> periodList = new List<PeriodCalendarDisplayCollection>();

        public tax_calendar()
        {
            ItemFilingPeriodList = new CollectionViewSource() { Source = list }.View;
            ItemFilingPeriodList.Filter = x => DoFilterList(x as TaxFilingPeriodViewModel);
            FilingPeriodsDayList = new CollectionViewSource() { Source = periodList }.View;

            Filters = new FilterGroup();
            Filters.PropertyChanged += Filters_PropertyChanged;
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            list.Clear();
            list.AddRange(TaxCalendarHelper.GetPeriodListAsync().GetResult());
            ItemFilingPeriodList.Refresh();

            taxCalendar_DisplayMonthChanged(null, null);

            ResetFilterList();
        }

        

        private IEnumerable<PeriodCalendarDisplayCollection> GetPeriodListByDisplayMonth(int year, DateTime startDisplayDate, DateTime endDisplayDate)
        {
            List<PeriodCalendarDisplay> periodDisplays = new List<PeriodCalendarDisplay>();
            foreach (var l in list.Where(l => l.IsActive))
            {
                periodDisplays.AddRange(l.GetPeriodDatesByYear(year)
                    .Where(d => d >= startDisplayDate && d <= endDisplayDate)
                    .Select(d => new PeriodCalendarDisplay() 
                    { 
                        PeriodDate = d, 
                        Item = l 
                    }));
            }
            return periodDisplays.GroupBy(i => i.PeriodDate)
                .Select(g => new PeriodCalendarDisplayCollection()
                {
                    Day = g.Key,
                    Items = g
                });
        }

        private void RefreshPeriodCalendarDisplay()
        {
            periodList.Clear();
            periodList.AddRange(GetPeriodListByDisplayMonth(taxCalendar.DisplayMonth.Year, taxCalendar.DisplayDateStart, taxCalendar.DisplayDateEnd));
            FilingPeriodsDayList.Refresh();
        }

        private void taxCalendar_DisplayMonthChanged(object sender, EventArgs e)
        {
            RefreshPeriodCalendarDisplay();
        }

        private bool DoFilterList(TaxFilingPeriodViewModel i)
        {
            bool flag = true;

            if (!Filters.FilterInactive && !i.IsActive) return false;

            //keyword
            if (!string.IsNullOrWhiteSpace(Filters.FilterKeyword))
            {
                string keyword = Filters.FilterKeyword.Trim();
                flag &= i.FormName.IndexOf(keyword, 0, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                    i.FormTitle.IndexOf(keyword, 0, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                    i.Description.IndexOf(keyword, 0, StringComparison.InvariantCultureIgnoreCase) >= 0;
            }

            //category
            if (Filters.FilterCategory != 0)
            {
                flag &= i.Category == null ? false : (i.Category.Id == Filters.FilterCategory);
            }

            return flag;
        }

        private void Filters_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ItemFilingPeriodList.Refresh();
        }

        private void ResetFilterList()
        {
            var list = TaxCalendarHelper.GetCategoryListAsync().GetResult().ToDictionary(i => (int)i.Id, i => i.CategoryName);
            list.Add(0, "     ");
            list = list.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
            Filters.CategoryList = list;
        }

        private void AddPeriod_btn_Click(object sender, RoutedEventArgs e)
        {
            bool isEdit = true;
            string title = "Edit Form";
            var period = (sender as FrameworkElement).DataContext as TaxFilingPeriodViewModel;
            if (period == null)
            {
                isEdit = false;
                title = "Add Form";
                period = new TaxFilingPeriodViewModel();
            }
            var modal = new Templates.add_tax_filing_period();
            var clone = period.DeepClone();
            modal.DataContext = clone;
            if (ModalForm.ShowModal(modal, title, ModalButtons.SaveCancel) == ModalResult.Save)
            {
                clone.DeepCopyTo(period);
                _ = TaxCalendarHelper.AddUpdatePeriodAsync(period);
                if (!isEdit)
                    list.Insert(0, period);
                ItemFilingPeriodList.Refresh();
                RefreshPeriodCalendarDisplay();
            }
        }

        private void AddFormPayment_btn_Click(object sender, RoutedEventArgs e)
        {
            var formPayment = new TaxFilingPaymentViewModel();
            var periodDisplay = (sender as Button).DataContext as PeriodCalendarDisplay;
            var period = periodDisplay?.Item;
            var modal = new Templates.add_tax_filing_history();
            if (periodDisplay != null)
            {
                formPayment.FormName = period.FormName;
                formPayment.PeriodDate = periodDisplay.PeriodDate;
            }
            else
            {
                //manual adding
                modal.IsManual = true;
                period = (sender as FrameworkElement).DataContext as TaxFilingPeriodViewModel;
                formPayment.FormName = period.FormName;
                formPayment.PeriodDate = DateTime.Now;
            }
            formPayment.Period = period;
            modal.DataContext = formPayment;
            if (ModalForm.ShowModal(modal, "Add Form Payment", ModalButtons.SaveCancel) == ModalResult.Save)
            {
                formPayment.FilingDate = DateTime.Now;
                formPayment.UpdatedDate = DateTime.Now;
                _ = TaxCalendarHelper.AddFormPaymentAsync(formPayment);
            }
        }
    }
}
