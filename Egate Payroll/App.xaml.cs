using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Egate_Payroll.Classes;

namespace Egate_Payroll
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public const string ATTENDANCE_FILES_DIRECTORY = "CrossChex Attendance Files";

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            InitializeFolders();
            InitializeErrorHandling();
            InitializeDatabae();

            Window window = new MainWindow();
            window.Closed += Window_Closed;
            window.Show();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void InitializeFolders()
        {
            FileHelper.CreateDirectory(App.ATTENDANCE_FILES_DIRECTORY);
        }

        private void InitializeDatabae()
        {
            using (var context = new Egate_Payroll.Model.PayrollModel())
            {
                context.Initialize();
            }
            using (var context = new Egate_Payroll.Deductions.Model.PayrollDeductionsModel())
            {
                context.Initialize();
            }
            using (var context = new Egate_Payroll.Tax_Calendar.Model.TaxCalendarModel())
            {
                context.Initialize();
            }
        }

        private void InitializeErrorHandling()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            Exception ex = e.Exception;
            System.Diagnostics.Debug.WriteLine(ex.ToString());
            Logs.Exception(ex);
        }

        private void Current_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Exception ex = e.Exception;
            System.Diagnostics.Debug.WriteLine(ex.ToString());
            Logs.Exception(ex);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            System.Diagnostics.Debug.WriteLine(ex.ToString());
            Logs.Exception(ex);
        }
    }
}
