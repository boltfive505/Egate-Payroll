using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Data.Entity;
using Egate_Payroll.Model;
using Egate_Payroll.Objects;
using Egate_Payroll.Classes;
using System.ComponentModel;

namespace Egate_Payroll.Pages
{
    /// <summary>
    /// Interaction logic for history_payroll.xaml
    /// </summary>
    public partial class history_payroll : Page
    {
        public class FilterDetailsGroup : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            public int? FilterCutoff { get; set; }
            public string FilterDepartment { get; set; }
            public string FilterName { get; set; }

            public List<string> DepartmentList { get; set; } = new List<string>();
            public List<string> NameList { get; set; } = new List<string>();
        }

        public static readonly DependencyProperty ItemCutoffListProperty = DependencyProperty.Register(nameof(ItemCutoffList), typeof(ICollectionView), typeof(history_payroll));
        public ICollectionView ItemCutoffList
        { 
            get { return (ICollectionView)GetValue(ItemCutoffListProperty); }
            set { SetValue(ItemCutoffListProperty, value); }
        }

        public static readonly DependencyProperty ItemDetailsListProperty = DependencyProperty.Register(nameof(ItemDetailsList), typeof(ICollectionView), typeof(history_payroll));
        public ICollectionView ItemDetailsList
        {
            get { return (ICollectionView)GetValue(ItemDetailsListProperty); }
            set { SetValue(ItemDetailsListProperty, value); }
        }

        public static readonly DependencyProperty FilterDetailsProperty = DependencyProperty.Register(nameof(FilterDetails), typeof(FilterDetailsGroup), typeof(history_payroll));
        public FilterDetailsGroup FilterDetails
        {
            get { return (FilterDetailsGroup)GetValue(FilterDetailsProperty); }
            set { SetValue(FilterDetailsProperty, value); }
        }

        public static readonly DependencyProperty CurrentPeriodProperty = DependencyProperty.Register(nameof(CurrentPeriod), typeof(CutoffViewModel), typeof(history_payroll));
        public CutoffViewModel CurrentPeriod
        {
            get { return (CutoffViewModel)GetValue(CurrentPeriodProperty); }
            set
            {
                SetValue(CurrentPeriodProperty, value);
                OnCurrentPeriodChanged();
            }
        }

        public static readonly RoutedEvent SetCurrentPeriodEvent = EventManager.RegisterRoutedEvent(nameof(SetCurrentPeriod), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(history_payroll));
        public event RoutedEventHandler SetCurrentPeriod
        {
            add { AddHandler(SetCurrentPeriodEvent, value); }
            remove { RemoveHandler(SetCurrentPeriodEvent, value); }
        }

        public static readonly RoutedEvent RefreshCurrentPeriod1Event = EventManager.RegisterRoutedEvent(nameof(RefreshCurrentPeriod1), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(history_payroll));
        public event RoutedEventHandler RefreshCurrentPeriod1
        {
            add { AddHandler(RefreshCurrentPeriod1Event, value); }
            remove { RemoveHandler(RefreshCurrentPeriod1Event, value); }
        }

        public static readonly RoutedEvent RefreshCurrentPeriod2Event = EventManager.RegisterRoutedEvent(nameof(RefreshCurrentPeriod2), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(history_payroll));
        public event RoutedEventHandler RefreshCurrentPeriod2
        {
            add { AddHandler(RefreshCurrentPeriod2Event, value); }
            remove { RemoveHandler(RefreshCurrentPeriod2Event, value); }
        }

        private List<CutoffViewModel> cutoffList = new List<CutoffViewModel>();
        private List<EmployeeWorkTimeObject> workList = new List<EmployeeWorkTimeObject>();
        private bool canSelect = false;

        public history_payroll()
        {
            FilterDetails = new FilterDetailsGroup();
            FilterDetails.PropertyChanged += Filters_PropertyChanged;
            ItemCutoffList = new CollectionViewSource() { Source = cutoffList }.View;
            ItemDetailsList = new CollectionViewSource() { Source = workList }.View;
            ItemDetailsList.Filter = x => DoFilterDetails(x as EmployeeWorkTimeObject);
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            canSelect = false;
            int selectedPeriodId = -1;
            if (CurrentPeriod != null)
            {
                selectedPeriodId = CurrentPeriod.Id;
            }
            cutoffList.Clear();
            cutoffList.AddRange(Helpers.GetCutoffList());
            CurrentPeriod = cutoffList.FirstOrDefault(i => i.Id == selectedPeriodId);
            if (CurrentPeriod != null)
                CurrentPeriod.IsSelected = true;
            ItemCutoffList.Refresh();
            canSelect = true;

            workList.Clear();
            workList.AddRange(Helpers.GetWorkList());
            ItemDetailsList.Refresh();
            SetFilterList();
        }

        private void Filters_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ItemDetailsList.Refresh();
        }

        private void SetFilterList()
        {
            FilterDetails.DepartmentList.Clear();
            FilterDetails.DepartmentList.AddRange(workList.Select(l => l.Department).Distinct().Where(x => !string.IsNullOrWhiteSpace(x.Trim())).OrderBy(x => x));
            FilterDetails.DepartmentList.Insert(0, "     ");
            FilterDetails.NameList.Clear();
            FilterDetails.NameList.AddRange(workList.Select(l => l.EmployeeName).Distinct().Where(x => !string.IsNullOrWhiteSpace(x.Trim())).OrderBy(x => x));
            FilterDetails.NameList.Insert(0, "     ");
        }

        private bool DoFilterDetails(EmployeeWorkTimeObject i)
        {
            bool flag = true;

            //cutoff selection
            if (FilterDetails.FilterCutoff == null) return false;
            if (FilterDetails.FilterCutoff != null)
            {
                flag &= i.CutoffId == (int)FilterDetails.FilterCutoff;
            }

            //department
            if (!string.IsNullOrWhiteSpace(FilterDetails.FilterDepartment))
                flag &= i.Department == FilterDetails.FilterDepartment;

            //employee name
            if (!string.IsNullOrWhiteSpace(FilterDetails.FilterName))
                flag &= i.EmployeeName == FilterDetails.FilterName;

            return flag;
        }

        private void CutoffList_Selected(object sender, RoutedEventArgs e)
        {
            FilterDetails.FilterCutoff = ((sender as FrameworkElement).DataContext as CutoffViewModel).Id;
        }

        private void DeleteCutoff_btn_Click(object sender, RoutedEventArgs e)
        {
            var cutoffVm = ((sender as FrameworkElement).DataContext as CutoffViewModel);
            if (MessageBox.Show(string.Format("Confirm DELETING of this period record {0:yyyy-MM-dd} - {1:yyyy-MM-dd}?", cutoffVm.StartDate, cutoffVm.EndDate), "Attendance History", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                if (CurrentPeriod == cutoffVm)
                {
                    CurrentPeriod = null;
                }
                cutoffList.Remove(cutoffVm);
                ItemCutoffList.Refresh();
                Task.Run(async () =>
                {
                    using (var context = new PayrollModel())
                    {
                        context.Database.Log = x => System.Diagnostics.Debug.WriteLine(x);
                        cutoff cu = await context.cutoff.Include("shift_settings").FirstOrDefaultAsync(i => i.Id == cutoffVm.Id);
                        if (cu != null)
                        {
                            context.cutoff.Remove(cu);
                            await context.SaveChangesAsync();
                        }
                    }
                });
                MessageBox.Show("Period record deleted", "Attendance History");
            }
        }

        private void OnCurrentPeriodChanged()
        {
            if (!canSelect) return;
            RaiseEvent(new RoutedEventArgs(SetCurrentPeriodEvent, this));
        }

        private void PeriodSelected_chk_Click(object sender, RoutedEventArgs e)
        {
            if (!canSelect) return;
            CheckBox chk = sender as CheckBox;
            if (chk.IsChecked != null && chk.IsChecked.Value)
            {
                if (CurrentPeriod != null)
                {
                    CurrentPeriod.IsSelected = false;
                }
                CurrentPeriod = (sender as FrameworkElement).DataContext as CutoffViewModel;
            }
        }

        private void EditRegularHours_btn_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPeriod == null) return;
            EditRegularHours_value.Value = CurrentPeriod.RegularHours;
            EditRegularHours_popup.IsOpen = true;
            if (EditRegularHours_popup.IsSubmitted)
            {
                CurrentPeriod.RegularHours = EditRegularHours_value.Value;
                using (var context = new PayrollModel())
                {
                    var cutoff = context.cutoff.FirstOrDefault(i => i.Id == CurrentPeriod.Id);
                    if (cutoff != null)
                    {
                        cutoff.RegularHours = CurrentPeriod.RegularHours;
                        context.SaveChanges();
                    }
                }
                RaiseEvent(new RoutedEventArgs(RefreshCurrentPeriod1Event, this));
            }
        }
    }
}
