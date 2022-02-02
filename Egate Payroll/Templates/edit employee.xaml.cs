using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Xceed.Wpf.Toolkit;
using CustomControls.Modal;

namespace Egate_Payroll.Templates
{
    /// <summary>
    /// Interaction logic for edit_employee.xaml
    /// </summary>
    public partial class edit_employee : UserControl, IModalClosed
    {
        public edit_employee()
        {
            InitializeComponent();
            EmployeeTypeValue.ItemsSource = Enum.GetValues(typeof(EmployeeType));
        }

        public void ModalClosed(ModalClosedArgs e)
        {
            if (e.Result == ModalResult.Save)
            {
                EmployeeNameValue.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                EmployeeTypeValue.GetBindingExpression(ComboBox.SelectedItemProperty).UpdateSource();
                MonthlyRateValue.GetBindingExpression(DecimalUpDown.ValueProperty).UpdateSource();
                HourlyRateValue.GetBindingExpression(DecimalUpDown.ValueProperty).UpdateSource();
                MealAllowanceValue.GetBindingExpression(DecimalUpDown.ValueProperty).UpdateSource();
                TransportationAllowanceValue.GetBindingExpression(DecimalUpDown.ValueProperty).UpdateSource();
                OtherAllowanceValue.GetBindingExpression(DecimalUpDown.ValueProperty).UpdateSource();
                HasDeductionsValue.GetBindingExpression(CheckBox.IsCheckedProperty).UpdateSource();
                NotesValue.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            }
        }
    }
}
