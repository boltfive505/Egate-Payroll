using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Data.Entity;
using Egate_Payroll.Objects.TaxCalendar;
using Egate_Payroll.Classes;
using CustomControls.Modal;

namespace Egate_Payroll.Pages
{
    /// <summary>
    /// Interaction logic for employee_list.xaml
    /// </summary>
    public partial class tax_company_list : Page
    {
        public static readonly DependencyProperty CompanyListProperty = DependencyProperty.Register(nameof(CompanyList), typeof(ICollectionView), typeof(employee_list));
        public ICollectionView CompanyList
        {
            get { return (ICollectionView)GetValue(CompanyListProperty); }
            set { SetValue(CompanyListProperty, value); }
        }

        private List<TaxFilingCompanyViewModel> list = new List<TaxFilingCompanyViewModel>();

        public tax_company_list()
        {
            CompanyList = new CollectionViewSource() { Source = list }.View;
            CompanyList.SortDescriptions.Add(new SortDescription("CompanyName", ListSortDirection.Ascending));
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            list.Clear();
            list.AddRange(TaxCalendarHelper.GetCompanyListAsync().GetResult());
            CompanyList.Refresh();
        }

        private void AddCompany_Click(object sender, RoutedEventArgs e)
        {
            bool isEdit = true;
            string title = "Edit Tax Company";
            TaxFilingCompanyViewModel company = (sender as FrameworkElement).DataContext as TaxFilingCompanyViewModel;
            if (company == null)
            {
                isEdit = false;
                title = "Add Tax Company";
                company = new TaxFilingCompanyViewModel();
            }
            var modal = new Templates.tax_company_add_modal();
            var clone = company.DeepClone();
            modal.DataContext = clone;
            if (ModalForm.ShowModal(modal, title, ModalButtons.SaveCancel) == ModalResult.Save)
            {
                clone.DeepCopyTo(company);
                _ = TaxCalendarHelper.AddCompanyAsync(company);
                if (!isEdit)
                    list.Add(company);
                CompanyList.Refresh();
            }
        }
    }
}
