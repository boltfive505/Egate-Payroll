using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Globalization;
using Xceed.Wpf.Toolkit;
using CustomControls.Modal;
using Egate_Payroll.Classes;
using Egate_Payroll.Tax_Calendar.Model;

namespace Egate_Payroll.Templates
{
    /// <summary>
    /// Interaction logic for add_tax_filing_period.xaml
    /// </summary>
    public partial class add_tax_filing_period : UserControl, IModalClosed
    {
        public add_tax_filing_period()
        {
            InitializeComponent();

            //period type
            PeriodTypeValue.ItemsSource = Enum.GetValues(typeof(FilingPeriodType));
            //due month
            DueMonthValue.ItemsSource = Enumerable.Range(1, 12).Select(n => new DateTime(1, n, 1)).ToDictionary(i => i.Month, i => i.ToString("MMMM", CultureInfo.InvariantCulture));
            DueMonthValue.DisplayMemberPath = "Value";
            DueMonthValue.SelectedValuePath = "Key";
            DueMonthValue.SelectionChanged += DueMonthValue_SelectionChanged;
            //annual month due days  
            LoadAnnualMonthDueDays(1);
            //monthly due days
            DueDaysValue.ItemsSource = Enumerable.Range(1, 31);
            //load categories
            using (var context = new TaxCalendarModel())
            {
                FilingCategoryValue.ItemsSource = context.filing_category.ToList();
            }
            FilingCategoryValue.DisplayMemberPath = "CategoryName";
            FilingCategoryValue.SelectedValuePath = "Id";
        }

        private void LoadAnnualMonthDueDays(int month)
        {
            DueDaysValue2.ItemsSource = Enumerable.Range(1, DateTime.DaysInMonth(1, month));
            DueDaysValue2.SelectedItem = 1;
        }

        private void DueMonthValue_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cbox = sender as ComboBox;
            if (cbox.SelectedValue != null)
            {
                int month = (int)cbox.SelectedValue;
                LoadAnnualMonthDueDays(month);
            }
        }

        private static void CheckBoxListUpdateSource(ListBox lstBox)
        {
            var chkBoxes = VisualHelper.FindVisualChildren<CheckBox>(lstBox);
            foreach (var chk in chkBoxes)
                chk.GetBindingExpression(CheckBox.IsCheckedProperty).UpdateSource();
        }

        public void ModalClosed(ModalClosedArgs e)
        {
            if (e.Result == ModalResult.Save)
            {
                FormNameValue.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                DescriptionValue.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                FormTitleValue.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                FilingCategoryValue.GetBindingExpression(ComboBox.SelectedValueProperty).UpdateSource();
                PeriodTypeValue.GetBindingExpression(ComboBox.SelectedItemProperty).UpdateSource();
                IsActiveValue.GetBindingExpression(CheckBox.IsCheckedProperty).UpdateSource();
                switch ((FilingPeriodType)PeriodTypeValue.SelectedItem)
                {
                    case FilingPeriodType.OneTime:
                        DueDateStartValue.GetBindingExpression(DateTimePicker.ValueProperty).UpdateSource();
                        break;
                    case FilingPeriodType.Monthly:
                        DueDaysValue.GetBindingExpression(ComboBox.SelectedItemProperty).UpdateSource();
                        CheckBoxListUpdateSource(MonthInclusionListValue);
                        break;
                    case FilingPeriodType.EndOfQuarter:
                        DueDaysValue3.GetBindingExpression(IntegerUpDown.ValueProperty).UpdateSource();
                        CheckBoxListUpdateSource(QuarterInclusionListValue);
                        break;
                    case FilingPeriodType.Annually:
                        DueMonthValue.GetBindingExpression(ComboBox.SelectedValueProperty).UpdateSource();
                        DueDaysValue2.GetBindingExpression(ComboBox.SelectedItemProperty).UpdateSource();
                        break;
                }
            }
        }
    }
}
