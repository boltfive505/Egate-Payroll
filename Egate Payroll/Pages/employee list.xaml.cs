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
using Egate_Payroll.Objects;
using Egate_Payroll.Classes;
using Egate_Payroll.Model;
using CustomControls.Modal;

namespace Egate_Payroll.Pages
{
    /// <summary>
    /// Interaction logic for employee_list.xaml
    /// </summary>
    public partial class employee_list : Page
    {
        public static readonly DependencyProperty EmployeeListProperty = DependencyProperty.Register(nameof(EmployeeList), typeof(ICollectionView), typeof(employee_list));
        public ICollectionView EmployeeList
        {
            get { return (ICollectionView)GetValue(EmployeeListProperty); }
            set { SetValue(EmployeeListProperty, value); }
        }

        private List<EmployeeViewModel> list = new List<EmployeeViewModel>();

        public employee_list()
        {
            EmployeeList = new CollectionViewSource() { Source = list }.View;

            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            list.Clear();
            list.AddRange(GetList());
            _ = SetEmployeeYtdInfoAsync(list);
            EmployeeList.Refresh();
        }

        private IEnumerable<EmployeeViewModel> GetList()
        {
            using (var context = new PayrollModel())
            {
                context.Configuration.AutoDetectChangesEnabled = false;
                var query = from emp in context.employee
                            select emp;
                return query.ToList()
                    .Select(i => new EmployeeViewModel()
                    {
                        EmployeeId = (int)i.Id,
                        EmployeeNumber = (int)i.EmployeeNumber,
                        EmployeeName = i.EmployeeName,
                        Department = i.Department,
                        EmployeeType = i.EmployeeType.ToEnum<EmployeeType>(),
                        DateHired = i.DateHired.ToUnixDate(),
                        MonthlyRate = i.MonthlyRate,
                        HourlyRate = i.HourlyRate,
                        MealAllowance = i.MealAllowance,
                        TransportationAllowance = i.TransportationAllowance,
                        OtherAllowance = i.OtherAllowance,
                        HasDeductions = i.HasDeductions.ToBool(),
                        Notes = i.Notes,
                        IsActive = i.IsActive.ToBool()
                    })
                    .OrderBy(i => i.EmployeeName);
            }
        }

        private async Task SetEmployeeYtdInfoAsync(IEnumerable<EmployeeViewModel> employeeList)
        {
            using (var context = new PayrollModel())
            {
                context.Configuration.AutoDetectChangesEnabled = false;
                DateTime now = DateTime.Now;
                decimal startDate = new DateTime(now.Year, 1, 1, 0, 0, 0).ToUnixLong();
                decimal endDate = new DateTime(now.Year, 12, 31, 23, 59, 59).ToUnixLong();
                //get summary based on start cover period
                var query = from attSummary in context.attendance_summary
                            join cu in context.cutoff on attSummary.CutoffId equals cu.Id
                            where cu.StartDate >= startDate && cu.StartDate <= endDate
                            group attSummary by attSummary.EmployeeId into g
                            select g;
                var employeeAttendanceSummaryList = await query.ToListAsync();
                foreach (var emp in employeeList)
                {
                    var employeeAttendanceSummary = employeeAttendanceSummaryList.FirstOrDefault(g => g.Key == emp.EmployeeId);
                    if (employeeAttendanceSummary != null)
                    {
                        emp.YearToDateGrossIncome = employeeAttendanceSummary.Sum(i => i.GrossIncome ?? 0);
                        emp.YearToDateAllowanceIncome = employeeAttendanceSummary.Sum(i => i.AllowanceIncome ?? 0);
                    }
                }
            }
        }

        private void EditEmployee_btn_Click(object sender, RoutedEventArgs e)
        {
            var editEmployee = (sender as FrameworkElement).DataContext as EmployeeViewModel;
            var modal = new Templates.edit_employee();
            modal.DataContext = editEmployee;
            if (ModalForm.ShowModal(modal, "Edit Employee", ModalButtons.SaveCancel) == ModalResult.Save)
            {
                //save to database
                Task.Run(async () =>
                {
                    using (var context = new PayrollModel())
                    {
                        var employee = await context.employee.FirstOrDefaultAsync(i => i.EmployeeNumber == editEmployee.EmployeeNumber);
                        if (employee != null)
                        {
                            employee.EmployeeName = editEmployee.EmployeeName;
                            employee.MonthlyRate = editEmployee.MonthlyRate;
                            employee.HourlyRate = editEmployee.HourlyRate;
                            employee.MealAllowance = editEmployee.MealAllowance;
                            employee.TransportationAllowance = editEmployee.TransportationAllowance;
                            employee.OtherAllowance = editEmployee.OtherAllowance;
                            employee.EmployeeType = editEmployee.EmployeeType.ToString();
                            employee.HasDeductions = editEmployee.HasDeductions.ToLong();
                            employee.Notes = editEmployee.Notes;
                            employee.IsActive = editEmployee.IsActive.ToLong();
                            await context.SaveChangesAsync();
                            Dispatcher.Invoke(() => RaiseEvent(new RoutedEventArgs(Pages.history_payroll.RefreshCurrentPeriod1Event, this)));
                        }
                    }
                });
            }
        }
    }
}
