using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CustomControls.Modal;
using Xceed.Wpf.Toolkit;

namespace Egate_Payroll.Templates.Contribution_Tables
{
    /// <summary>
    /// Interaction logic for pagibig_edit_bracket.xaml
    /// </summary>
    public partial class pagibig_edit_bracket : UserControl, IModalClosed
    {
        public pagibig_edit_bracket()
        {
            InitializeComponent();
        }

        public void ModalClosed(ModalClosedArgs e)
        {
            if (e.Result == ModalResult.Save)
            {
                CompensationFromValue.GetBindingExpression(DecimalUpDown.ValueProperty).UpdateSource();
                CompensationToValue.GetBindingExpression(DecimalUpDown.ValueProperty).UpdateSource();
                EmployeeShareRateValue.GetBindingExpression(DecimalUpDown.ValueProperty).UpdateSource();
                EmployerShareRateValue.GetBindingExpression(DecimalUpDown.ValueProperty).UpdateSource();
            }
        }
    }
}
