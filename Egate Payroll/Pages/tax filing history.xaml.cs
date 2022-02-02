using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Egate_Payroll.Tax_Calendar.Model;
using Egate_Payroll.Objects.TaxCalendar;
using Egate_Payroll.Classes;
using System.Data.Entity;
using System.IO;
using Microsoft.Win32;
using PropertyChanged;
using CustomControls.Modal;

namespace Egate_Payroll.Pages
{
    /// <summary>
    /// Interaction logic for tax_filing_history.xaml
    /// </summary>
    public partial class tax_filing_history : Page
    {
        public class FilterGroup : INotifyPropertyChanged
        {
            public class CategoryFormPair
            {
                public string CategoryName { get; set; }
                public string FormName { get; set; }
            }

            public event PropertyChangedEventHandler PropertyChanged;
            public string FilterFormName { get; set; }
            public string FilterCategoryName { get; set; }

            public ICollectionView ItemFormNameList { get; set; }
            public List<CategoryFormPair> CategoryFormNameList { get; set; } = new List<CategoryFormPair>();
            public List<string> CategoryNameList { get; set; } = new List<string>();

            public FilterGroup()
            {
                ItemFormNameList = new CollectionViewSource() { Source = CategoryFormNameList }.View;
                ItemFormNameList.Filter = x => DoFilterCategoryFormName(x as CategoryFormPair);
            }

            private bool DoFilterCategoryFormName(CategoryFormPair i)
            {
                bool flag = true;

                if (!string.IsNullOrWhiteSpace(FilterCategoryName))
                {
                    flag &= i.CategoryName == FilterCategoryName.Trim() || i.FormName == null;
                }

                return flag;
            }

            private void OnFilterCategoryNameChanged()
            {
                ItemFormNameList.Refresh();
                FilterFormName = string.Empty;
            }
        }

        public static readonly DependencyProperty ItemPaymentHistoryListProperty = DependencyProperty.Register(nameof(ItemPaymentHistoryList), typeof(ICollectionView), typeof(tax_filing_history));
        public ICollectionView ItemPaymentHistoryList
        {
            get { return (ICollectionView)GetValue(ItemPaymentHistoryListProperty); }
            set { SetValue(ItemPaymentHistoryListProperty, value); }
        }

        public static readonly DependencyProperty FiltersProperty = DependencyProperty.Register(nameof(Filters), typeof(FilterGroup), typeof(tax_filing_history));
        public FilterGroup Filters
        {
            get { return (FilterGroup)GetValue(FiltersProperty); }
            set { SetValue(FiltersProperty, value); }
        }

        private List<TaxFilingPaymentViewModel> paymentList = new List<TaxFilingPaymentViewModel>();

        public tax_filing_history()
        {
            ItemPaymentHistoryList = new CollectionViewSource() { Source = paymentList }.View;
            ItemPaymentHistoryList.Filter = x => DoFilterPaymentList(x as TaxFilingPaymentViewModel);

            Filters = new FilterGroup();
            Filters.PropertyChanged += Filters_PropertyChanged;
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            paymentList.Clear();
            paymentList.AddRange(TaxCalendarHelper.GetPaymentListAsync().GetResult());
            ItemPaymentHistoryList.Refresh();

            ResetFilterList();
        }

        private void Filters_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ItemPaymentHistoryList.Refresh();
        }

        private bool DoFilterPaymentList(TaxFilingPaymentViewModel i)
        {
            bool flag = true;

            //form
            if (!string.IsNullOrWhiteSpace(Filters.FilterFormName))
                flag &= i.FormName == Filters.FilterFormName;

            //department
            if (!string.IsNullOrWhiteSpace(Filters.FilterCategoryName))
                flag &= i.CategoryName == Filters.FilterCategoryName;

            return flag;
        }

        private void ResetFilterList()
        {
            Filters.CategoryFormNameList.Clear();
            Filters.CategoryNameList.Clear();

            using (var context = new TaxCalendarModel())
            {
                //form names
                var query1 = (from form in context.filing_form
                                   join cat in context.filing_category on form.FilingCategoryId equals cat.Id
                                   select new { cat.CategoryName, form.FormName });
                var categoryFormNameList = query1.ToList().Select(i => new FilterGroup.CategoryFormPair() { CategoryName = i.CategoryName, FormName = i.FormName }).OrderBy(i => i.FormName).ToList();
                categoryFormNameList.Insert(0, new FilterGroup.CategoryFormPair() { CategoryName = "  ", FormName = null });
                Filters.CategoryFormNameList.AddRange(categoryFormNameList);
                Filters.ItemFormNameList.Refresh();
                //categories
                var categoryNameList = (from cat in context.filing_category
                                             select cat.CategoryName).ToList();
                categoryNameList = categoryNameList.OrderBy(x => x).ToList();
                categoryNameList.Insert(0, "");
                Filters.CategoryNameList.AddRange(categoryNameList);
            }
        }

        private void OpenFormFile(string formFileName)
        {
            if (string.IsNullOrEmpty(formFileName)) return;
            string file = FileHelper.GetFile(formFileName, "uploads\\bir files");
            if (File.Exists(file))
            {
                FileHelper.Open(file);
            }
            else
            {
                MessageBox.Show("ERROR: File not found", "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditPayment_btn_Click(object sender, RoutedEventArgs e)
        {
            var editPayment = (sender as FrameworkElement).DataContext as TaxFilingPaymentViewModel;
            var modal = new Templates.add_tax_filing_history();
            modal.IsManual = true;
            var clone = editPayment.DeepClone();
            modal.DataContext = clone;
            ModalResult result = ModalForm.ShowModal(modal, "Edit Form Payment", ModalButtons.SaveCancelDelete);
            if (result == ModalResult.Save)
            {
                clone.DeepCopyTo(editPayment);
                editPayment.UpdatedDate = DateTime.Now;
                _ = TaxCalendarHelper.AddFormPaymentAsync(editPayment);
                ItemPaymentHistoryList.Refresh();
            }
            else if (result == ModalResult.Delete)
            {
                _ = TaxCalendarHelper.DeleteFormPaymentAsync(editPayment);
                paymentList.Remove(editPayment);
                ItemPaymentHistoryList.Refresh();
            }
        }

        private bool Formname_cbox_SearchText(object item, string searchText)
        {
            var i = item as FilterGroup.CategoryFormPair;
            return i.FormName != null && i.FormName.IndexOf(searchText.Trim(), 0, StringComparison.InvariantCultureIgnoreCase) >= 0;
        }

        private void OpenTaxFile_Click(object sender, RoutedEventArgs e)
        {
            string file = FileHelper.GetFile(FileExtension.GetFileName(sender as FrameworkElement), TaxCalendarHelper.BIR_FILE);
            if (File.Exists(file))
                FileHelper.Open(file);
            else
                MessageBox.Show("ERROR: File not found", "", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
