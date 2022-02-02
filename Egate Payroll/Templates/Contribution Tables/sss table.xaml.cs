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
    /// Interaction logic for sss_table.xaml
    /// </summary>
    public partial class sss_table : UserControl
    {
        public sss_table()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this)) return;
            sssDg.ItemsSource = GetList();
        }

        private IEnumerable<sss> GetList()
        {
            using (var context = new PayrollDeductionsModel())
            {
                context.Configuration.AutoDetectChangesEnabled = false;
                return context.sss.AsNoTracking().ToList();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var editSss = (sender as FrameworkElement).DataContext as sss;
            var editBracket = new sss_edit_bracket();
            editBracket.DataContext = editSss;
            if (ModalForm.ShowModal(editBracket, "Edit SSS Bracket", ModalButtons.SaveCancel) == ModalResult.Save)
            {
                Task.Run(async () =>
                {
                    using (var deductions = new PayrollDeductionsModel())
                    {
                        sss sss = await deductions.sss.FirstOrDefaultAsync(i => i.Id == editSss.Id);
                        if (sss != null)
                        {
                            sss.CompensationFrom = editSss.CompensationFrom;
                            sss.CompensationTo = editSss.CompensationTo;
                            sss.EmployeeContribution = editSss.EmployeeContribution;
                            sss.EmployerContribution = editSss.EmployerContribution;
                            await deductions.SaveChangesAsync();
                        }
                    }
                });
            }
        }
    }
}
