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
    /// Interaction logic for summary.xaml
    /// </summary>
    public partial class summary : UserControl
    {
        public static readonly DependencyProperty ItemListProperty = DependencyProperty.Register(nameof(ItemListSource), typeof(ICollectionView), typeof(summary));
        public ICollectionView ItemListSource
        {
            get { return (ICollectionView)GetValue(ItemListProperty); }
            set { SetValue(ItemListProperty, value); }
        }

        public summary()
        {
            InitializeComponent();
        }
    }
}
