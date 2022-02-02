using Egate_Payroll.Model;
using Egate_Payroll.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Data.Entity;

namespace Egate_Payroll.Templates
{
    /// <summary>
    /// Interaction logic for holiday_calendar.xaml
    /// </summary>
    public partial class holiday_calendar : UserControl
    {
        public class OptionGroup : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            public bool RegularHolidayAllowOvertime { get; set; }
            public bool SpecialHolidayAllowOvertime { get; set; }
        }

        public static readonly DependencyProperty HolidayItemListProperty = DependencyProperty.Register(nameof(HolidayItemList), typeof(ICollectionView), typeof(holiday_calendar));
        public ICollectionView HolidayItemList
        {
            get { return (ICollectionView)GetValue(HolidayItemListProperty); }
            set { SetValue(HolidayItemListProperty, value); }
        }

        public static readonly DependencyProperty OptionsProperty = DependencyProperty.Register(nameof(Options), typeof(OptionGroup), typeof(holiday_calendar));
        public OptionGroup Options
        {
            get { return (OptionGroup)GetValue(OptionsProperty); }
            set { SetValue(OptionsProperty, value); }
        }

        private List<HolidayViewModel> list = new List<HolidayViewModel>();

        public holiday_calendar()
        {
            HolidayItemList = new CollectionViewSource() { Source = list }.View;
            Options = new OptionGroup();
            Options.PropertyChanged += Options_PropertyChanged;

            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this)) return;
            list.Clear();
            list.AddRange(GetHolidayList());
            HolidayItemList.Refresh();

            GetOptions();
        }

        private IEnumerable<HolidayViewModel> GetHolidayList()
        {
            using (var context = new PayrollModel())
            {
                context.Configuration.AutoDetectChangesEnabled = false;
                return context.holiday.AsNoTracking().ToList()
                    .Select(i => new HolidayViewModel()
                    {
                        Id = (int)i.Id,
                        HolidayType = i.HolidayType.ToEnum<HolidayType>(),
                        Day = i.Date.ToUnixDate(),
                        OtherName = i.OtherName,
                        OtherRate = i.OtherRate
                    });
            }
        }

        private void GetOptions()
        {
            var t = Task.Run<Egate_Payroll.Model.options>(async () =>
            {
                using (var context = new PayrollModel())
                {
                    context.Configuration.AutoDetectChangesEnabled = false;
                    var opt = await context.options.FirstOrDefaultAsync();
                    return opt;
                }
            });
            t.ContinueWith(result =>
            {
                var opt = result.Result;
                Dispatcher.Invoke(() => 
                {
                    Options.RegularHolidayAllowOvertime = opt.RegularHolidayAllowOvertime.ToBool();
                    Options.SpecialHolidayAllowOvertime = opt.SpecialHolidayAllowOvertime.ToBool();
                });
            });
        }

        private void Options_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // save changes to database
            using (var context = new PayrollModel())
            {
                var opt = context.options.First();
                opt.RegularHolidayAllowOvertime = Options.RegularHolidayAllowOvertime.ToLong();
                opt.SpecialHolidayAllowOvertime = Options.SpecialHolidayAllowOvertime.ToLong();
                context.SaveChanges();
            }
            RaiseEvent(new RoutedEventArgs(Pages.history_payroll.RefreshCurrentPeriod1Event, this));
        }

        private void MonthlyCalendar_DayClick(object sender, CustomMonthlyCalendar.DayClickEventArgs e)
        {
            holidayListbox.SelectedItem = DependencyProperty.UnsetValue;
            var tempList = Enum.GetValues(typeof(HolidayType)).OfType<HolidayType>()
                .Select(t => new HolidayOptionItem() { HolidayType = t })
                .ToList();
            holidayListbox.ItemsSource = tempList;
            //if holiday exist, do editing
            HolidayViewModel hvm = list.FirstOrDefault(h => h.Day == e.Day);
            if (hvm != null)
            {
                var item = tempList.First(i => i.HolidayType == hvm.HolidayType);
                holidayListbox.SelectedItem = item;
                item.OtherName = hvm.OtherName;
                item.OtherRate = hvm.OtherRate;
            }

            editingHolidayDay.Text = e.Day.ToShortDateString();
            selectHolidayPopup.IsOpen = true;
            if (selectHolidayPopup.IsSubmitted)
            {
                HolidayOptionItem option = holidayListbox.SelectedItem as HolidayOptionItem;
                if (option != null)
                {
                    HolidayViewModel holidayVm = list.FirstOrDefault(h => h.Day == e.Day);
                    if (holidayVm == null)
                    {
                        holidayVm = new HolidayViewModel() { Day = e.Day };
                        list.Add(holidayVm);
                    }
                    holidayVm.HolidayType = option.HolidayType;
                    holidayVm.OtherName = option.OtherName;
                    holidayVm.OtherRate = option.OtherRate;
                    HolidayItemList.Refresh();
                    var t = SaveHolidayToDatabaseAsync(holidayVm);
                    RaiseEvent(new RoutedEventArgs(Pages.history_payroll.RefreshCurrentPeriod1Event, this));
                }
            }
        }

        private async Task SaveHolidayToDatabaseAsync(HolidayViewModel hvm)
        {
            using (var context = new PayrollModel())
            {
                long day = hvm.Day.Date.ToUnixLong();
                var holiday = context.holiday.FirstOrDefault(h => h.Date == day);
                if (holiday == null)
                {
                    holiday = new holiday() { Date = day };
                    context.holiday.Add(holiday);
                }
                holiday.HolidayType = hvm.HolidayType.ToString();
                holiday.OtherName = hvm.OtherName;
                holiday.OtherRate = hvm.OtherRate;
                await context.SaveChangesAsync();
                hvm.Id = (int)holiday.Id;
            }
        }
    }
}
