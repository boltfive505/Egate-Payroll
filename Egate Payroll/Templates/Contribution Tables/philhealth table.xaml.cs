using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Data.Entity;
using Egate_Payroll.Deductions.Model;
using System.ComponentModel;
using CustomControls.Modal;

namespace Egate_Payroll.Templates.Contribution_Tables
{
    /// <summary>
    /// Interaction logic for philhealth_table.xaml
    /// </summary>
    public partial class philhealth_table : UserControl
    {
        public philhealth_table()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this)) return;
            philhealthDg.ItemsSource = GetList();
        }

        private IEnumerable<philhealth> GetList()
        {
            using (var deductions = new PayrollDeductionsModel())
            {
                deductions.Configuration.AutoDetectChangesEnabled = false;
                return deductions.philhealth.AsNoTracking().ToList();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var ph = (sender as FrameworkElement).DataContext as philhealth;
            var editTable = new philhealth_edit_bracket();
            editTable.DataContext = ph;
            if (ModalForm.ShowModal(editTable, "Edit PhilHealth Bracket", ModalButtons.SaveCancel) == ModalResult.Save)
            {
                Task.Run(async () =>
                {   
                    using (var deductions = new PayrollDeductionsModel())
                    {
                        philhealth philhealth = await deductions.philhealth.FirstOrDefaultAsync(i => i.Id == ph.Id);
                        if (philhealth != null)
                        {
                            philhealth.MonthlyBasicSalaryFrom = ph.MonthlyBasicSalaryFrom;
                            philhealth.MonthlyBasicSalaryTo = ph.MonthlyBasicSalaryTo;
                            philhealth.PremiumRate = ph.PremiumRate;
                            await deductions.SaveChangesAsync();
                        }
                    }
                });
            }
        }
    }
}
