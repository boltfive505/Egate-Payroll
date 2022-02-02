using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Egate_Payroll.Deductions.Model;
using System.Data.Entity;
using System.ComponentModel;
using CustomControls.Modal;

namespace Egate_Payroll.Templates.Contribution_Tables
{
    /// <summary>
    /// Interaction logic for pagibig_table.xaml
    /// </summary>
    public partial class pagibig_table : UserControl
    {
        public pagibig_table()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this)) return;
            pagibigDg.ItemsSource = GetList();
        }

        private IEnumerable<pagibig> GetList()
        {
            using (var context = new PayrollDeductionsModel())
            {
                context.Configuration.AutoDetectChangesEnabled = false;
                return context.pagibig.AsNoTracking().ToList();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var editPagIbig = (sender as FrameworkElement).DataContext as pagibig;
            var editBracket = new pagibig_edit_bracket();
            editBracket.DataContext = editPagIbig;
            if (ModalForm.ShowModal(editBracket, "Edit Pag-Ibig Bracket", ModalButtons.SaveCancel) == ModalResult.Save)
            {
                Task.Run(async () =>
                {
                    using (var deductions = new PayrollDeductionsModel())
                    {
                        pagibig pagibig = await deductions.pagibig.FirstOrDefaultAsync(i => i.Id == editPagIbig.Id);
                        if (pagibig != null)
                        {
                            pagibig.MonthlyCompensationFrom = editPagIbig.MonthlyCompensationFrom;
                            pagibig.MonthlyCompensationTo = editPagIbig.MonthlyCompensationTo;
                            pagibig.EmployeeShareRate = editPagIbig.EmployeeShareRate;
                            pagibig.EmployerShareRate = editPagIbig.EmployerShareRate;
                            await deductions.SaveChangesAsync();
                        }
                    }
                });
            }
        }
    }
}
