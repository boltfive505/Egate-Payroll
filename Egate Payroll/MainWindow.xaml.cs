using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.IO;
using System.Reflection;
using Egate_Payroll.Classes;
using Egate_Payroll.Objects;
using Egate_Payroll.Pages;

namespace Egate_Payroll
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static readonly DependencyProperty SelectedTabProperty = DependencyProperty.Register(nameof(SelectedTab), typeof(object), typeof(MainWindow));
        public object SelectedTab
        {
            get { return GetValue(SelectedTabProperty); }
            set { SetValue(SelectedTabProperty, value); }
        }

        public MainWindow()
        {
            InitializeComponent();
            SelectedTab = importAttendance_tab;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void import_attendance_SavedData(object sender, RoutedEventArgs e)
        {
            SelectedTab = savedAttendance_tab;
        }

        private void TabControl_SetCurrentPeriod(object sender, RoutedEventArgs e)
        {
            var currentPeriod = historyPayroll.CurrentPeriod;
            computation1.CurrentPeriod = currentPeriod;
            //computation2.CurrentPeriod = currentPeriod;
            computationSummary.CurrentPeriod = currentPeriod;
        }

        private void TabControl_RefreshCurrentPeriod1(object sender, RoutedEventArgs e)
        {
            computation1.LoadCurrentPeriod();
            computationSummary.LoadCurrentPeriod();
            computationSummary.SaveSummaryInfo();
        }

        private void TabControl_RefreshCurrentPeriod2(object sender, RoutedEventArgs e)
        {
            computationSummary.LoadCurrentPeriod();
            computationSummary.SaveSummaryInfo();
        }
    }
}
