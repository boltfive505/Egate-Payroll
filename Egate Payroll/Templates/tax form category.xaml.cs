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
using CustomControls.Modal;
using Egate_Payroll.Tax_Calendar.Model;

namespace Egate_Payroll.Templates
{
    /// <summary>
    /// Interaction logic for tax_form_category.xaml
    /// </summary>
    public partial class tax_form_category : UserControl
    {
        public class CategoryViewModel : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            public int Id { get; set; } // database reference
            public string CategoryName { get; set; }

            public string Description { get; set; }
        }


        public static readonly DependencyProperty ItemCategoryListProperty = DependencyProperty.Register(nameof(ItemCategoryList), typeof(ICollectionView), typeof(tax_form_category));
        public ICollectionView ItemCategoryList
        {
            get { return (ICollectionView)GetValue(ItemCategoryListProperty); }
            set { SetValue(ItemCategoryListProperty, value); }
        }

        private List<CategoryViewModel> list = new List<CategoryViewModel>();

        public tax_form_category()
        {
            ItemCategoryList = new CollectionViewSource() { Source = list }.View;

            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this)) return;
            list.Clear();
            list.AddRange(GetList());
            ItemCategoryList.Refresh();
        }

        private IEnumerable<CategoryViewModel> GetList()
        {
            using (var context = new TaxCalendarModel())
            {
                context.Configuration.AutoDetectChangesEnabled = false;
                return context.filing_category.ToList()
                    .Select(i => new CategoryViewModel()
                    {
                        Id = (int)i.Id,
                        CategoryName = i.CategoryName,
                        Description = i.Description
                    });
            }
        }

        private void AddCategory_btn_Click(object sender, RoutedEventArgs e)
        {
            var category = new CategoryViewModel();
            var modal = new Templates.add_tax_form_category();
            modal.DataContext = category;
            if (ModalForm.ShowModal(modal, "Add Category", ModalButtons.SaveCancel) == ModalResult.Save)
            {
                _ = AddUpdateCategoryAsync(category);
                list.Add(category);
                ItemCategoryList.Refresh();
            }
        }

        private void EditCategory_btn_Click(object sender, RoutedEventArgs e)
        {
            var category = (sender as FrameworkElement).DataContext as CategoryViewModel;
            var modal = new Templates.add_tax_form_category();
            modal.DataContext = category;
            if (ModalForm.ShowModal(modal, "Edit Category", ModalButtons.SaveCancel) == ModalResult.Save)
            {
                _ = AddUpdateCategoryAsync(category);
            }
        }

        private async Task AddUpdateCategoryAsync(CategoryViewModel newCategory)
        {
            using (var context = new TaxCalendarModel())
            {
                var category = await context.filing_category.FirstOrDefaultAsync(i => i.Id == newCategory.Id);
                if (category == null)
                {
                    category = new filing_category();
                    context.filing_category.Add(category);
                }
                category.CategoryName = newCategory.CategoryName;
                category.Description = newCategory.Description;

                await context.SaveChangesAsync();

                if (newCategory.Id != category.Id)
                    newCategory.Id = (int)category.Id;
            }
        }
    }
}
