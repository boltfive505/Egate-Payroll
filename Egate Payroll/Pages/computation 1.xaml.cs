using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Data.Entity;
using Egate_Payroll.Classes;
using Egate_Payroll.Objects;
using Egate_Payroll.Model;
using Xceed.Wpf.Toolkit;

namespace Egate_Payroll.Pages
{
    /// <summary>
    /// Interaction logic for settings_payroll.xaml
    /// </summary>
    public partial class computation_1 : Page
    {
        public class FilterGroup : INotifyPropertyChanged
        {
            public class DetailedName : INotifyPropertyChanged
            {
                public event PropertyChangedEventHandler PropertyChanged;
                public string Name { get; set; }
                public TimeSpan? TotalTime { get; set; }

                public string DisplayName { get { return string.IsNullOrEmpty(Name) ? "- All Employees -" : Name; } }

                public override bool Equals(object obj)
                {
                    if (obj is DetailedName)
                    {
                        var o = obj as DetailedName;
                        return this.Name == o.Name;
                    }
                    return false;
                }
                //public string Display { get { return string.IsNullOrEmpty(Name) ? "- All Employees -" : string.Format("{0} ({1})", Name, new Converters.TimeSpanTotalHoursDisplayConverter().Convert(TotalTime, null, null, null)); } }
            }

            public event PropertyChangedEventHandler PropertyChanged;
            public string FilterName { get; set; }
            public string FilterDepartment { get; set; }
            public DateTime? FilterFromDate { get; set; }
            public DateTime? FilterToDate { get; set; }
            public bool FilterManualTimeInput { get; set; }

            public DateTime? MinDate { get; set; }
            public DateTime? MaxDate { get; set; }

            public List<DetailedName> DetailedNameList { get; set; } = new List<DetailedName>();
            public List<string> DepartmentList { get; set; } = new List<string>();

            public bool CanFilter { get; set; }

            public void Reset()
            {
                CanFilter = false;
                FilterName = string.Empty;
                FilterDepartment = string.Empty;
                FilterFromDate = null;
                FilterToDate = null;
                FilterFromDate = MinDate;
                FilterToDate = MaxDate;
                FilterManualTimeInput = false;
                CanFilter = true;
            }

            public void Clear()
            {
                MinDate = null;
                MaxDate = null;
                Reset();
            }
        }

        public class TotalGroup : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            public TimeSpan TotalWorkHours { get; set; }
            public TimeSpan TotalOvertimeHours { get; set; }
            public TimeSpan TotalWorkTotal { get; set; }
        }

        public static readonly DependencyProperty ItemCutoffListProperty = DependencyProperty.Register(nameof(ItemCutoffList), typeof(ICollectionView), typeof(computation_1));
        public ICollectionView ItemCutoffList
        {
            get { return (ICollectionView)GetValue(ItemCutoffListProperty); }
            set { SetValue(ItemCutoffListProperty, value); }
        }

        public static readonly DependencyProperty ItemComputedListProperty = DependencyProperty.Register(nameof(ItemComputedList), typeof(ICollectionView), typeof(computation_1));
        public ICollectionView ItemComputedList
        {
            get { return (ICollectionView)GetValue(ItemComputedListProperty); }
            set { SetValue(ItemComputedListProperty, value); }
        }

        public static readonly DependencyProperty SettingsProperty = DependencyProperty.Register(nameof(Settings), typeof(ShiftSettingsViewModel), typeof(computation_1));
        public ShiftSettingsViewModel Settings
        {
            get { return (ShiftSettingsViewModel)GetValue(SettingsProperty); }
            set { SetValue(SettingsProperty, value); }
        }

        public static readonly DependencyProperty CurrentPeriodProperty = DependencyProperty.Register(nameof(CurrentPeriod), typeof(CutoffViewModel), typeof(computation_1));
        public CutoffViewModel CurrentPeriod
        {
            get { return (CutoffViewModel)GetValue(CurrentPeriodProperty); }
            set 
            {
                SetValue(CurrentPeriodProperty, value);
                OnCurrentPeriodChanged();
            }
        }

        public static readonly DependencyProperty FiltersProperty = DependencyProperty.Register(nameof(Filters), typeof(FilterGroup), typeof(computation_1));
        public FilterGroup Filters
        { 
            get { return (FilterGroup)GetValue(FiltersProperty); }
            set { SetValue(FiltersProperty, value); }
        }

        private List<CutoffViewModel> cutoffList = new List<CutoffViewModel>();
        private List<EmployeeComputedPayrollViewModel> list = new List<EmployeeComputedPayrollViewModel>();
        private bool currentPeriodFlag = true;
        private TotalGroup totals;

        public computation_1()
        {
            InitializeComponent();

            for (int i = 0; i < attendanceDataGrid.Columns.Count; i++)
            {
                Binding colBind = new Binding("Columns[" + i + "].ActualWidth");
                colBind.ElementName = "attendanceDataGrid";
                ColumnDefinition col = new ColumnDefinition();
                col.SetBinding(ColumnDefinition.WidthProperty, colBind);
                totalsGrid.ColumnDefinitions.Add(col);
            }
            totals = new TotalGroup();
            totalsGrid.DataContext = totals;

            ItemCutoffList = new CollectionViewSource() { Source = cutoffList }.View;
            ItemComputedList = new CollectionViewSource() { Source = list }.View;
            ItemComputedList.CollectionChanged += ItemComputedList_CollectionChanged;
            ItemComputedList.Filter = x => DoFilterList(x as EmployeeComputedPayrollViewModel);
            Filters = new FilterGroup();
            Filters.PropertyChanged += Filters_PropertyChanged;

            Settings = new ShiftSettingsViewModel();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            int selectedPeriodId = -1;
            if (CurrentPeriod != null)
                selectedPeriodId = CurrentPeriod.Id;
            cutoffList.Clear();
            cutoffList.AddRange(Helpers.GetCutoffList());
            ItemCutoffList.Refresh();
            currentPeriodFlag = false;
            CurrentPeriod = cutoffList.FirstOrDefault(i => i.Id == selectedPeriodId);
            if (CurrentPeriod != null)
                CurrentPeriod.IsSelected = true;
            currentPeriodFlag = true;
        }

        private void ApplyAndSave_btn_Click(object sender, RoutedEventArgs e)
        {
            foreach (var l in list)
                l.SetComputation1(Settings);

            var tasks = new Task[] {
                DoSaveComputationToDatabaseAsync(list),
                DoSaveShiftSettingsToDatabaeAsync(this.Settings, this.CurrentPeriod.Id)
            };
            try
            {
                Task.Run(async () =>
                {
                    await Task.WhenAll(tasks);
                    Dispatcher.Invoke(() => RaiseEvent(new RoutedEventArgs(Pages.history_payroll.RefreshCurrentPeriod2Event, this)));
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        private async Task DoSaveComputationToDatabaseAsync(List<EmployeeComputedPayrollViewModel> computedList)
        {
            var tasks = from l in computedList
                        group l by l.EmployeeNumber into g
                        select Task.Run(async () =>
                        {
                            using (var context = new PayrollModel())
                            {
                                foreach (var computed in g)
                                {
                                    try
                                    {
                                        var att = context.attendance.FirstOrDefault(i => i.Id == computed.AttendanceId);
                                        if (att != null)
                                        {
                                            att.ComputedTimeIn = computed.ComputedTimeIn.ToUnixLong();
                                            att.ComputedTimeOut = computed.ComputedTimeOut.ToUnixLong();
                                            att.ComputedLateHours = computed.ComputedLate.ToTotalHours();
                                            att.ComputedEarlyHours = computed.ComputedEarly.ToTotalHours();
                                            att.ComputedWorkTimeHours = computed.ComputedWorkTime.ToTotalHours();
                                            att.ComputedOvertimeHours = computed.ComputedOvertime.ToTotalHours();
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        System.Diagnostics.Debug.WriteLine(ex.ToString());
                                    }
                                }
                                await context.SaveChangesAsync();
                            }
                        });
            await Task.WhenAll(tasks);
        }

        private async Task DoSaveShiftSettingsToDatabaeAsync(ShiftSettingsViewModel ssvm, int cutoffId)
        {
            using (var context = new PayrollModel())
            {
                try
                {
                    var query = from cu in context.cutoff
                                join shift in context.shift_settings on cu.Id equals shift.CutoffId
                                where cu.Id == cutoffId
                                select shift;
                    var shiftSettings = query.FirstOrDefault();
                    if (shiftSettings == null)
                    {
                        shiftSettings = new shift_settings();
                        shiftSettings.CutoffId = cutoffId;
                        context.shift_settings.Add(shiftSettings);
                    }
                    DateTime min = DateTimeExt.MinUnixDate;
                    shiftSettings.OnDuty = new DateTime(min.Year, min.Month, min.Day, ssvm.OnDuty.Hour, ssvm.OnDuty.Minute, 0).ToUnixLong();
                    shiftSettings.OffDuty = new DateTime(min.Year, min.Month, min.Day, ssvm.OffDuty.Hour, ssvm.OffDuty.Minute, 0).ToUnixLong();
                    shiftSettings.OvertimeOffset = ssvm.OvertimeOffsetMinutes;
                    shiftSettings.IsDeductOvertimeOffset = ssvm.DeductOvertimeOffset.ToLong();
                    await context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                }
            }
        }

        private void ItemComputedList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            ComputeTotals();
        }

        private void ComputeTotals()
        {
            var filteredComputedList = ItemComputedList.OfType<EmployeeComputedPayrollViewModel>();
            totals.TotalWorkHours = TimeSpan.FromHours(filteredComputedList.Sum(i => i.ActualWorkTime?.TotalHours ?? 0));
            totals.TotalOvertimeHours = TimeSpan.FromHours(filteredComputedList.Sum(i => i.ActualWorkOvertime?.TotalHours ?? 0));
            totals.TotalWorkTotal = TimeSpan.FromHours(filteredComputedList.Sum(i => i.TotalWorkTime?.TotalHours ?? 0));
        }

        private void EditTimeIn_btn_Click(object sender, RoutedEventArgs e)
        {
            //edit time in
            EmployeeComputedPayrollViewModel computed = ((sender as FrameworkElement).DataContext as EmployeeComputedPayrollViewModel);
            EditTime(computed, computed.TimeIn, (c, time) => c.TimeIn = time, (att, time) => att.TimeIn = time.ToUnixLong());
        }

        private void EditTimeOut_btn_Click(object sender, RoutedEventArgs e)
        {
            //edit time out
            EmployeeComputedPayrollViewModel computed = ((sender as FrameworkElement).DataContext as EmployeeComputedPayrollViewModel);
            EditTime(computed, computed.TimeOut, (c, time) => c.TimeOut = time, (att, time) => att.TimeOut = time.ToUnixLong());
        }

        private void EditTime(EmployeeComputedPayrollViewModel computed, DateTime? value, Action<EmployeeComputedPayrollViewModel, DateTime?> set1, Action<attendance, DateTime?> set2)
        {
            editTime_timepicker.Value = value;
            editTime_popup.IsOpen = true;
            if (editTime_popup.IsSubmitted)
            {
                DateTime? editedTime = editTime_timepicker.Value;
                DateTime now = DateTime.Now;
                set1(computed, editedTime);
                computed.ModifyTimeDate = now;
                computed.SetComputation1(Settings);

                //save to database
                Task.Run(async () =>
                {
                    using (var context = new PayrollModel())
                    {
                        var att = context.attendance.FirstOrDefault(i => i.Id == computed.AttendanceId);
                        if (att != null)
                        {
                            set2(att, editedTime);
                            att.ModifyTimeDate = now.ToUnixLong();
                        }
                        await context.SaveChangesAsync();
                    }
                    await DoSaveComputationToDatabaseAsync(new List<EmployeeComputedPayrollViewModel>() { computed });
                    Dispatcher.Invoke(() => RaiseEvent(new RoutedEventArgs(Pages.history_payroll.RefreshCurrentPeriod2Event, this)));
                });
            }
        }

        private void SelectPeriod_btn_Click(object sender, RoutedEventArgs e)
        {
            CurrentPeriod = (sender as FrameworkElement).DataContext as CutoffViewModel;
        }

        public void LoadCurrentPeriod()
        {
            //set list
            list.Clear();
            if (CurrentPeriod != null)
            {
                list.AddRange(Helpers.GetComputedList(CurrentPeriod.Id));
            }
            ItemComputedList.Refresh();
            //set shift
            if (CurrentPeriod != null)
            {
                Settings = Helpers.GetShiftSettings(CurrentPeriod.Id);
            }
            ResetFilterList();
        }

        private void OnCurrentPeriodChanged()
        {
            if (!currentPeriodFlag) return;
            LoadCurrentPeriod();
        }

        private void ResetFilterList()
        {
            Filters.Reset();

            Filters.DetailedNameList = list.GroupBy(i => i.EmployeeNumber)
                .Select(g => new FilterGroup.DetailedName()
                {
                    Name = g.First().EmployeeName,
                    TotalTime = (TimeSpan?)TimeSpan.FromHours(g.Where(x => x.TotalTime != null).Sum(x => x.TotalTime.Value.TotalHours))
                }).ToList();
            Filters.DetailedNameList.Insert(0, new FilterGroup.DetailedName());
            Filters.DepartmentList = list.Select(i => i.Department).Distinct().OrderBy(x => x).ToList();
            Filters.DepartmentList.Insert(0, "   ");

            if (list != null && list.Count > 0)
            {
                Filters.MinDate = list.Select(i => i.WorkDate).Min();
                Filters.MaxDate = list.Select(i => i.WorkDate).Max();
                Filters.FilterFromDate = Filters.MinDate;
                Filters.FilterToDate = Filters.MaxDate;
            }
        }

        private bool DoFilterList(EmployeeComputedPayrollViewModel i)
        {
            bool flag = true;

            //employee name
            if (!string.IsNullOrWhiteSpace(Filters.FilterName))
                flag &= i.EmployeeName == Filters.FilterName;

            //department
            if (!string.IsNullOrWhiteSpace(Filters.FilterDepartment))
                flag &= i.Department == Filters.FilterDepartment;

            //date
            if (Filters.FilterFromDate.HasValue && Filters.FilterToDate.HasValue)
            {
                DateTime from = (DateTime)Filters.FilterFromDate;
                from = new DateTime(from.Year, from.Month, from.Day, 0, 0, 0);
                DateTime to = (DateTime)Filters.FilterToDate;
                to = new DateTime(to.Year, to.Month, to.Day, 23, 59, 59);
                flag &= i.WorkDate >= from && i.WorkDate <= to;
            }

            //manual time input
            if (Filters.FilterManualTimeInput)
            {
                flag &= i.ModifyTimeDate != null;
            }

            return flag;
        }

        private void ResetFilter_btn_Click(object sender, RoutedEventArgs e)
        {
            Filters.Reset();
            ItemComputedList.Refresh();
        }

        private void Filters_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (Filters.CanFilter)
                ItemComputedList.Refresh();
        }

        private void ApplyDateRange_btn_Click(object sender, RoutedEventArgs e)
        {
            fromDateRange.GetBindingExpression(DateTimePicker.ValueProperty).UpdateSource();
            toDateRange.GetBindingExpression(DateTimePicker.ValueProperty).UpdateSource();
        }

        private void SetAdjustmentTime_btn_Click(object sender, RoutedEventArgs e)
        {
            var computed = (sender as FrameworkElement).DataContext as EmployeeComputedPayrollViewModel;

            adjustmentHours_popup.DataContext = computed;
            adjustmentHours_popup.IsOpen = true;
            if (adjustmentHours_popup.IsSubmitted)
            {
                if (adjustmentHours_popup.IsDeleting)
                {
                    //delete
                    computed.AdjustmentTime = null;
                    computed.AdjustmentTimeMode = AdjustedHoursMode.Add;
                    computed.AdjustmentTimeNotes = string.Empty;
                }
                else
                {
                    //add / update
                    AdjustmentTimeValue.GetBindingExpression(TimeSpanUpDown.ValueProperty).UpdateSource();
                    AdjustmentTimeMode.GetBindingExpression(ListBox.SelectedItemProperty).UpdateSource();
                    AdjustmentTimeNotes.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                }
                //save to database
                Task.Run(async () =>
                {
                    using (var context = new PayrollModel())
                    {
                        var attendance = await context.attendance.FirstOrDefaultAsync(i => i.Id == computed.AttendanceId);
                        if (attendance != null)
                        {
                            attendance.AdjustedHours = computed.AdjustmentTime.ToTotalHours();
                            attendance.AdjustedHoursMode = computed.AdjustmentTimeMode.ToString();
                            attendance.AdjustedHoursNotes = computed.AdjustmentTimeNotes;
                            await context.SaveChangesAsync();
                            Dispatcher.Invoke(() => RaiseEvent(new RoutedEventArgs(Pages.history_payroll.RefreshCurrentPeriod2Event, this)));
                        }
                    }
                });
            }
        }

        private bool Employees_cbox_SearchText(object item, string searchText)
        {
            if (item != null)
            {
                var i = item as FilterGroup.DetailedName;
                if (!string.IsNullOrEmpty(i.Name))
                {
                    return i.Name.IndexOf(searchText, 0, StringComparison.InvariantCultureIgnoreCase) >= 0;
                }
            }
            return false;
        }

        private void ExportExcel_btn_Click(object sender, RoutedEventArgs e)
        {
            var filteredList = ItemComputedList.OfType<EmployeeComputedPayrollViewModel>();
            if (filteredList.Count() > 0)
            {
                Excel_Reports.ExportEmployeeComputedAttendance.ExportList(filteredList);
            }
        }
    }
}
