using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.IO;
using System.ComponentModel;
using System.Windows.Data;

namespace Egate_Payroll.Pages
{
    /// <summary>
    /// Interaction logic for test.xaml
    /// </summary>
    public partial class test : Page
    {
        public class FilterGroup : INotifyPropertyChanged
        {
            public class CategoryFormPair
            {
                public string CategoryName { get; set; }
                public string FormName { get; set; }
            }

            public event PropertyChangedEventHandler PropertyChanged;
            public ICollectionView ItemList { get; set; }
            public string FilterText { get; set; }

            public List<CategoryFormPair> CategoryFormNameList { get; set; } = new List<CategoryFormPair>();

            public FilterGroup()
            {
                ItemList = new CollectionViewSource() { Source = CategoryFormNameList }.View;
                ItemList.Filter = x => DoFilterItem(x as CategoryFormPair);
            }

            private bool DoFilterItem(CategoryFormPair i)
            {
                bool flag = true;
                if (!string.IsNullOrWhiteSpace(FilterText))
                {
                    //flag &= i.CategoryName.IndexOf(FilterText.Trim(), 0, StringComparison.InvariantCultureIgnoreCase) >= 0;
                }
                return flag;
            }

            private void OnFilterTextChanged()
            {
                ItemList.Refresh();
            }
        }

        public static readonly DependencyProperty FiltersProperty = DependencyProperty.Register(nameof(Filters), typeof(FilterGroup), typeof(test));
        public FilterGroup Filters
        {
            get { return (FilterGroup)GetValue(FiltersProperty); }
            set { SetValue(FiltersProperty, value); }
        }

        private List<string> list = new List<string>();

        public test()
        {
            Filters = new FilterGroup();
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Filters.CategoryFormNameList.Clear();
            Filters.CategoryFormNameList.Add(new FilterGroup.CategoryFormPair() { CategoryName = "SSS", FormName = "1906-M" });
            Filters.CategoryFormNameList.Add(new FilterGroup.CategoryFormPair() { CategoryName = "SSS", FormName = "2020" });
            Filters.CategoryFormNameList.Add(new FilterGroup.CategoryFormPair() { CategoryName = "BIR", FormName = "9018-Q" });
            Filters.CategoryFormNameList.Add(new FilterGroup.CategoryFormPair() { CategoryName = "BIR", FormName = "1414-CMRM" });


            Filters.ItemList.Refresh();
            //list.Clear();
            //list.Add("12345");
            //list.Add("test");
            //list.Add("asd");
            //list.Add("poi");
            //ItemList.Refresh();
        }

        private bool DoFilterItem(string i)
        {
            bool flag = true;
            //if (!string.IsNullOrWhiteSpace(FilterText))
            //{
            //    flag &= i.IndexOf(FilterText.Trim(), 0, StringComparison.InvariantCultureIgnoreCase) >= 0;
            //}
            return flag;
        }

        private static void OnFilterTextPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            //if (sender is test)
            //{
            //    (sender as test).ItemList.Refresh();
            //}
        }
    }
}
