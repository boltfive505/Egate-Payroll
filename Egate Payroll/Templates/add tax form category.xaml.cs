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

namespace Egate_Payroll.Templates
{
    /// <summary>
    /// Interaction logic for add_tax_form_category.xaml
    /// </summary>
    public partial class add_tax_form_category : UserControl, IModalClosed
    {
        public add_tax_form_category()
        {
            InitializeComponent();
        }

        public void ModalClosed(ModalClosedArgs e)
        {
            if (e.Result == ModalResult.Save)
            {
                CategoryNameValue.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                DescriptionValue.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            }
        }
    }
}
