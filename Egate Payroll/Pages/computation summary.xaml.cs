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
using Egate_Payroll.Deductions.Model;
using System.IO;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Xml.Xsl;
using System.Xml;
using Xceed.Wpf.Toolkit;
using CustomControls.Modal;
using System.Collections.ObjectModel;

namespace Egate_Payroll.Pages
{
    /// <summary>
    /// Interaction logic for settings_payroll.xaml
    /// </summary>
    public partial class computation_summary : Page
    {
        public class FilterGroup : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            public string FilterName { get; set; }
            public string FilterDepartment { get; set; }
            public DateTime? FilterFromDate { get; set; }
            public DateTime? FilterToDate { get; set; }
            public bool FilterManualTimeInput { get; set; }

            public DateTime? MinDate { get; set; }
            public DateTime? MaxDate { get; set; }

            public List<string> NameList { get; set; } = new List<string>();
            public List<string> DepartmentList { get; set; } = new List<string>();

            public bool CanFilter { get; set; }

            public void Reset()
            {
                CanFilter = false;
                FilterName = string.Empty;
                FilterDepartment = string.Empty;
                FilterFromDate = MinDate;
                FilterToDate = MaxDate;
                FilterManualTimeInput = false;
                CanFilter = true;
            }
        }

        public class TotalGroup : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            public int TotalEmployees { get; set; }
            public decimal TotalNetIncome { get; set; }
            public TimeSpan TotalRegularHours { get; set; }
            public TimeSpan TotalOvertimeHours { get; set; }
            public ObservableCollection<TotalDeductionsGroup> TotalDeductions { get; set; } = new ObservableCollection<TotalDeductionsGroup>();
        }

        public class TotalDeductionsGroup : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            public string Name { get; set; }
            public decimal SssTotal { get; set; }
            public decimal PhilHealthTotal { get; set; }
            public decimal PagIbigTotal { get; set; }
            public decimal TaxTotal { get; set; }
        }

        public class AdjustmentAmountGroup : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            public decimal? Amount { get; set; }
            public string Notes { get; set; }
        }

        public static readonly DependencyProperty ItemCutoffListProperty = DependencyProperty.Register(nameof(ItemCutoffList), typeof(ICollectionView), typeof(computation_summary));
        public ICollectionView ItemCutoffList
        {
            get { return (ICollectionView)GetValue(ItemCutoffListProperty); }
            set { SetValue(ItemCutoffListProperty, value); }
        }

        public static readonly DependencyProperty ItemSummaryListProperty = DependencyProperty.Register(nameof(ItemSummaryList), typeof(ICollectionView), typeof(computation_summary));
        public ICollectionView ItemSummaryList
        {
            get { return (ICollectionView)GetValue(ItemSummaryListProperty); }
            set { SetValue(ItemSummaryListProperty, value); }
        }

        public static readonly DependencyProperty CurrentPeriodProperty = DependencyProperty.Register(nameof(CurrentPeriod), typeof(CutoffViewModel), typeof(computation_summary));
        public CutoffViewModel CurrentPeriod
        {
            get { return (CutoffViewModel)GetValue(CurrentPeriodProperty); }
            set 
            { 
                SetValue(CurrentPeriodProperty, value);
                OnCurrentPeriodChanged();
            }
        }

        public static readonly DependencyProperty FiltersProperty = DependencyProperty.Register(nameof(Filters), typeof(FilterGroup), typeof(computation_summary));
        public FilterGroup Filters
        { 
            get { return (FilterGroup)GetValue(FiltersProperty); }
            set { SetValue(FiltersProperty, value); }
        }

        private List<CutoffViewModel> cutoffList = new List<CutoffViewModel>();
        private List<EmployeeWorkSummaryViewModel> list = new List<EmployeeWorkSummaryViewModel>();
        private TotalGroup totals;
        private bool currentPeriodFlag = true;

        public computation_summary()
        {
            ItemCutoffList = new CollectionViewSource() { Source = cutoffList }.View;
            ItemSummaryList = new CollectionViewSource() { Source = list }.View;
            ItemSummaryList.Filter = x => DoFilterList(x as EmployeeWorkSummaryViewModel);
            Filters = new FilterGroup();
            Filters.PropertyChanged += Filters_PropertyChanged;

            InitializeComponent();

            totals = new TotalGroup();
            totalsGroup.DataContext = totals;
            totalsGroup2.DataContext = totals;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            int selectedPeriodId = -1;
            if (CurrentPeriod != null)
            {
                selectedPeriodId = CurrentPeriod.Id;
            }
            cutoffList.Clear();
            cutoffList.AddRange(Helpers.GetCutoffList());
            ItemCutoffList.Refresh();
            currentPeriodFlag = false;
            CurrentPeriod = cutoffList.FirstOrDefault(i => i.Id == selectedPeriodId);
            if (CurrentPeriod != null)
                CurrentPeriod.IsSelected = true;
            currentPeriodFlag = true;
            ComputeTotals2();
        }

        private void ComputeTotals()
        {
            totals.TotalEmployees = list.Count;
            totals.TotalNetIncome = list.Sum(i => i.NetPay);
            //deductions
            totals.TotalDeductions.Clear();
            totals.TotalDeductions.Add(new TotalDeductionsGroup()
            { 
                Name = "Employee",
                SssTotal = list.Sum(i => i.Deductions.SssEmployeeContribution),
                PhilHealthTotal = list.Sum(i => i.Deductions.PhilhealthEmployeeContribution),
                PagIbigTotal = list.Sum(i => i.Deductions.PagibigEmployeeContribution),
                TaxTotal = list.Sum(i => i.Deductions.TaxEmployeeContribution)
            });
            totals.TotalDeductions.Add(new TotalDeductionsGroup()
            {
                Name = "Employer",
                SssTotal = list.Sum(i => i.Deductions.SssEmployerContribution),
                PhilHealthTotal = list.Sum(i => i.Deductions.PhilhealthEmployerContribution),
                PagIbigTotal = list.Sum(i => i.Deductions.PagibigEmployerContribution),
                TaxTotal = 0
            });
        }

        private void ComputeTotals2()
        {
            var filteredSummaryList = ItemSummaryList.OfType<EmployeeWorkSummaryViewModel>();
            totals.TotalRegularHours = TimeSpan.FromHours(filteredSummaryList.Sum(i => i.FinalRegularHours?.TotalHours ?? 0));
            totals.TotalOvertimeHours = TimeSpan.FromHours(filteredSummaryList.Sum(i => i.FinalOvertime?.TotalHours ?? 0));
        }

        public void LoadCurrentPeriod()
        {
            //set list
            list.Clear();
            if (CurrentPeriod != null)
            {
                list.AddRange(Helpers.GetSummaryList(CurrentPeriod.Id, CurrentPeriod.RegularHours));
            }
            var t = Helpers.SetPayrollDeductionsAsync(list);
            t.ContinueWith((task) =>
            {
                Dispatcher.BeginInvoke(new Action(() => ComputeTotals()), System.Windows.Threading.DispatcherPriority.Background);
            });
            ItemSummaryList.Refresh();
            ResetFilterList();
        }

        public void SaveSummaryInfo()
        {
            //run task in parallel
            if (CurrentPeriod == null) return;
            int cutoffId = CurrentPeriod.Id;
            IEnumerable<Task> tasks = from summary in list
                                      select Task.Run(async () =>
                                      {
                                          using (var context = new PayrollModel())
                                          {
                                              attendance_summary attSummary = await context.attendance_summary.FirstOrDefaultAsync(i => i.EmployeeId == summary.EmployeeId && i.CutoffId == cutoffId);
                                              if (attSummary == null)
                                              {
                                                  attSummary = new attendance_summary();
                                                  attSummary.EmployeeId = summary.EmployeeId;
                                                  attSummary.CutoffId = cutoffId;
                                                  context.attendance_summary.Add(attSummary);
                                              }
                                              attSummary.FinalRegularHours = (decimal?)summary.FinalRegularHours?.TotalHours;
                                              attSummary.FinalOvertimeHours = (decimal?)summary.FinalOvertime?.TotalHours;
                                              attSummary.TotalHolidayHours = (decimal?)summary.TotalHolidayTime.TotalHours;
                                              attSummary.TotalHours = (decimal?)summary.TotalHours.TotalHours;
                                              attSummary.GrossIncome = summary.GrossIncome;
                                              attSummary.AllowanceIncome = summary.GrandTotalAllowance;
                                              attSummary.SssDeductionTotal = summary.Deductions.SssEmployeeContribution;
                                              attSummary.PhilhealthDeductionTotal = summary.Deductions.PhilhealthEmployeeContribution;
                                              attSummary.PagibigDeductionTotal = summary.Deductions.PagibigEmployeeContribution;
                                              attSummary.TaxDeductionTotal = summary.Deductions.TaxEmployeeContribution;
                                              attSummary.NetPay = summary.NetPay;

                                              await context.SaveChangesAsync();
                                          }
                                      });
            Task.Run(async () =>
            {
                await Task.WhenAll(tasks);
            });
        }

        private void OnCurrentPeriodChanged()
        {
            if (!currentPeriodFlag) return;
            LoadCurrentPeriod();
        }

        private void ResetFilterList()
        {
            Filters.Reset();

            Filters.NameList = list.Select(i => i.EmployeeName).Distinct().OrderBy(x => x).ToList();
            Filters.NameList.Insert(0, "   ");
            Filters.DepartmentList = list.Select(i => i.Department).Distinct().OrderBy(x => x).ToList();
            Filters.DepartmentList.Insert(0, "   ");
        }

        private bool DoFilterList(EmployeeWorkSummaryViewModel i)
        {
            bool flag = true;

            //employee name
            if (!string.IsNullOrWhiteSpace(Filters.FilterName))
                flag &= i.EmployeeName == Filters.FilterName;

            //department
            if (!string.IsNullOrWhiteSpace(Filters.FilterDepartment))
                flag &= i.Department == Filters.FilterDepartment;

            return flag;
        }

        private void ResetFilter_btn_Click(object sender, RoutedEventArgs e)
        {
            Filters.Reset();
            ItemSummaryList.Refresh();
            ComputeTotals2();
        }

        private void Filters_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (Filters.CanFilter)
            {
                ItemSummaryList.Refresh();
                ComputeTotals2();
            }
        }

        private void PrintPayslip(params EmployeeWorkSummaryViewModel[] summaryList)
        {
            string xml = Helpers.GenerateXmlFromEmployeeSummaryList(summaryList, CurrentPeriod);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            XslCompiledTransform xsl = new XslCompiledTransform();
            XmlReaderSettings settings = new XmlReaderSettings()
            {
                DtdProcessing = DtdProcessing.Parse
            };
            var uri = new Uri("pack://application:,,,/BIR & Payroll;component/resources/payslip stylesheet.xsl", UriKind.RelativeOrAbsolute);
            XmlReader reader = XmlReader.Create(Application.GetResourceStream(uri).Stream, settings);
            xsl.Load(reader);
            string tempFile = Path.GetTempFileName();
            XmlTextWriter writer = new XmlTextWriter(tempFile, Encoding.UTF8);
            xsl.Transform(doc, null, writer);
            var print = new Templates.printing();
            print.WebPagePath = tempFile;
            ModalForm.ShowModal(print, "Print Payslip");
        }

        private void PrintAllPayslip_btn_Click(object sender, RoutedEventArgs e)
        {
            PrintPayslip(this.list.ToArray());
        }

        private void PrintSinglePayslip_btn_Click(object sender, RoutedEventArgs e)
        {
            var summary = (sender as FrameworkElement).DataContext as EmployeeWorkSummaryViewModel;
            PrintPayslip(summary);
        }

        private void ShowComputation_btn_Click(object sender, RoutedEventArgs e)
        {
            var summary = (sender as FrameworkElement).DataContext as EmployeeWorkSummaryViewModel;
            showComputation_popup.DataContext = new { Summary = summary, CurrentPeriod };
            showComputation_popup.IsOpen = true;
        }

        private void AddAdjustmentAmount_btn_Click(object sender, RoutedEventArgs e)
        {
            var editSummary = (sender as FrameworkElement).DataContext as EmployeeWorkSummaryViewModel;
            var adjustmentGroup = new AdjustmentAmountGroup() { Amount = editSummary.AdjustmentAmount, Notes = "" };
            addAdjustmentAmount_popup.DataContext = adjustmentGroup;
            addAdjustmentAmount_popup.IsOpen = true;
            if (addAdjustmentAmount_popup.IsSubmitted)
            {
                if (addAdjustmentAmount_popup.IsDeleting)
                {
                    //remove
                    adjustmentGroup.Amount = null;
                }
                //set value to summary item
                editSummary.AdjustmentAmount = adjustmentGroup.Amount ?? 0;
                Dispatcher.BeginInvoke(new Action(() => ComputeTotals()), System.Windows.Threading.DispatcherPriority.Background);
                //save to database
                Task.Run(async () =>
                {
                    using (var context = new PayrollModel())
                    {
                        int cutoffId = -1;
                        Dispatcher.Invoke(() => cutoffId = CurrentPeriod.Id);
                        var attSummary = context.attendance_summary.FirstOrDefault(i => i.EmployeeId == editSummary.EmployeeId && i.CutoffId == cutoffId);
                        if (attSummary == null)
                        {
                            attSummary = new attendance_summary();
                            attSummary.EmployeeId = editSummary.EmployeeId;
                            attSummary.CutoffId = cutoffId;
                            context.attendance_summary.Add(attSummary);
                        }
                        attSummary.AdjustmentAmount = adjustmentGroup.Amount;
                        await context.SaveChangesAsync();
                    }
                });
            }
            addAdjustmentAmount_popup.DataContext = null;
        }

        private void ExportExcel_Click(object sender, RoutedEventArgs e)
        {
            ExcelExportHelper.ExportPayrollSummary(list, CurrentPeriod.StartDate, CurrentPeriod.EndDate);
        }
    }
}
