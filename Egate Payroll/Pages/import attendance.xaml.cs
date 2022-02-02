using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Egate_Payroll.Objects;
using Egate_Payroll.Classes;
using Egate_Payroll.Model;
using Microsoft.Win32;
using System.ComponentModel;
using System.Windows.Threading;

namespace Egate_Payroll.Pages
{
    /// <summary>
    /// Interaction logic for import.xaml
    /// </summary>
    public partial class import_attendance : Page
    {
        public static readonly DependencyProperty ItemDetailsListProperty = DependencyProperty.Register(nameof(ItemDetailsList), typeof(ICollectionView), typeof(import_attendance));
        public ICollectionView ItemDetailsList
        {
            get { return (ICollectionView)GetValue(ItemDetailsListProperty); }
            set { SetValue(ItemDetailsListProperty, value); }
        }

        public static readonly RoutedEvent SavedDataEvent = EventManager.RegisterRoutedEvent(nameof(SavedData), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(import_attendance));
        public event RoutedEventHandler SavedData
        {
            add { AddHandler(SavedDataEvent, value); }
            remove { RemoveHandler(SavedDataEvent, value); }
        }

        private List<EmployeeWorkTimeObject> workList = new List<EmployeeWorkTimeObject>();

        public import_attendance()
        {
            ItemDetailsList = new CollectionViewSource() { Source = workList }.View;
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private async Task SaveListToDatabaseAsync(IEnumerable<EmployeeWorkTimeObject> list)
        {
            //get cutoff
            cutoff cutoff = new cutoff()
            {
                StartDate = list.Select(i => i.WorkDate.Date).Min().ToUnixLong(),
                EndDate = list.Select(i => i.WorkDate.Date).Max().ToUnixLong(),
                ImportTime = DateTime.Now.ToUnixLong()
            };
            using (var context = new PayrollModel())
            {
                context.Configuration.AutoDetectChangesEnabled = false;
                context.cutoff.Add(cutoff);
                await context.SaveChangesAsync();
            }
            //put all savechanges in parallel tasks
            var tasks = from l in list
                        group l by l.EmployeeNumber into g
                        select Task.Run(async () =>
                        {
                            using (var context = new PayrollModel())
                            {
                                context.Configuration.AutoDetectChangesEnabled = false;

                                //also check if employee already exist -> use employee number
                                long employeeNumber = g.First().EmployeeNumber;
                                employee employee = context.employee.FirstOrDefault(emp => emp.EmployeeNumber == employeeNumber);
                                if (employee == null)
                                {
                                    employee = new employee();
                                    employee.EmployeeNumber = employeeNumber;
                                    employee.EmployeeName = g.First().EmployeeName;
                                    employee.Department = g.First().Department;
                                    employee.EmployeeType = EmployeeType.NonRegular.ToString();
                                    employee.DateHired = DateTime.Now.Date.ToUnixLong();
                                    context.employee.Add(employee);
                                }
                                //add attendance records
                                IEnumerable<attendance> attendances = g.Select(i => new attendance()
                                {
                                    employee = employee,
                                    CutoffId = cutoff.Id,
                                    WorkDate = i.WorkDate.ToUnixLong(),
                                    OnDuty = i.OnDuty.ToUnixLong(),
                                    OffDuty = i.OffDuty.ToUnixLong(),
                                    TimeIn = i.TimeIn.ToUnixLong(),
                                    TimeOut = i.TimeOut.ToUnixLong(),
                                    LateHours = i.Late.ToTotalHours(),
                                    EarlyHours = i.Early.ToTotalHours(),
                                    OvertimeHours = i.Overtime.ToTotalHours(),
                                    WorkTimeHours = i.WorkTime.ToTotalHours(),
                                    IsAbsent = i.IsAbsent.ToLong()
                                });
                                context.attendance.AddRange(attendances);
                                //add summary records
                                attendance_summary summary = new attendance_summary();
                                summary.employee = employee;
                                summary.CutoffId = cutoff.Id;
                                context.attendance_summary.Add(summary);

                                await context.SaveChangesAsync();
                            }
                        });
            await Task.WhenAll(tasks);
        }

        private void ImportAttendance_btn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Title = "Select Attendance File";
            open.Filter = "Excel File|*.xls;*.xlsx";
            open.InitialDirectory = FileHelper.CreateDirectory(App.ATTENDANCE_FILES_DIRECTORY);
            if (open.ShowDialog() == true)
            {
                workList.Clear();
                workList.AddRange(Helpers.GetWorkTimeListFromFile(open.FileName));
                ItemDetailsList.Refresh();
            }
        }

        private void SaveAttendance_btn_Click(object sender, RoutedEventArgs e)
        {
            if (workList == null || workList.Count == 0) return;

            DispatcherFrame frame = new DispatcherFrame();
            var t = SaveListToDatabaseAsync(workList);
            t.ContinueWith((task) =>
            {
                frame.Continue = false;
                Dispatcher.Invoke(() =>
                {
                    MessageBox.Show("Attendance Saved", "", MessageBoxButton.OK, MessageBoxImage.Information);
                    ClearData_btn_Click(null, null);
                    RaiseEvent(new RoutedEventArgs(SavedDataEvent));
                });
            });
            Dispatcher.PushFrame(frame);
        }

        private void ClearData_btn_Click(object sender, RoutedEventArgs e)
        {
            workList.Clear();
            ItemDetailsList.Refresh();
        }
    }
}
