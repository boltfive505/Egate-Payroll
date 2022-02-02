using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Egate_Payroll.Templates
{
    /// <summary>
    /// Interaction logic for details.xaml
    /// </summary>
    public partial class details : UserControl
    {
        public static readonly DependencyProperty ItemListSourceProperty = DependencyProperty.Register(nameof(ItemListSource), typeof(ICollectionView), typeof(details));
        public ICollectionView ItemListSource
        {
            get { return (ICollectionView)GetValue(ItemListSourceProperty); }
            set { SetValue(ItemListSourceProperty, value); }
        }

        public details()
        {
            InitializeComponent();
        }
    }
}
