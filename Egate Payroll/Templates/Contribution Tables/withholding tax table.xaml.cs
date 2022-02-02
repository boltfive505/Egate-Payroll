using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using CustomControls.Modal;
using Egate_Payroll.Deductions.Model;
using System.Data.Entity;
using System.ComponentModel;

namespace Egate_Payroll.Templates.Contribution_Tables
{
    /// <summary>
    /// Interaction logic for withholding_tax_table.xaml
    /// </summary>
    public partial class withholding_tax_table : UserControl
    {
        public withholding_tax_table()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this)) return;
            taxDg.ItemsSource = GetList();
        }

        private IEnumerable<tax> GetList()
        {
            using (var context = new PayrollDeductionsModel())
            {
                context.Configuration.AutoDetectChangesEnabled = false;
                return context.tax.ToList();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var editTax = (sender as FrameworkElement).DataContext as tax;
            var editBracket = new withholding_tax_edit_bracket();
            editBracket.DataContext = editTax;
            if (ModalForm.ShowModal(editBracket, "Edit Tax Bracket", ModalButtons.SaveCancel) == ModalResult.Save)
            {
                Task.Run(async () =>
                {
                    using (var deductions = new PayrollDeductionsModel())
                    {
                        tax tax = await deductions.tax.FirstOrDefaultAsync(i => i.Id == editTax.Id);
                        if (tax != null)
                        {
                            tax.CompensationFrom = editTax.CompensationFrom;
                            tax.CompensationTo = editTax.CompensationTo;
                            tax.WithholdingTaxFixed = editTax.WithholdingTaxFixed;
                            tax.WithholdingTaxAdditionalRate = editTax.WithholdingTaxAdditionalRate;
                            await deductions.SaveChangesAsync();
                        }
                    }
                });
            }
        }
    }
}
