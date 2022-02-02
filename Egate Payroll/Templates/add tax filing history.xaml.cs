using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.IO;
using CustomControls;
using CustomControls.Modal;
using System.Collections.ObjectModel;
using Egate_Payroll.Classes;
using Egate_Payroll.Objects.TaxCalendar;

namespace Egate_Payroll.Templates
{
    /// <summary>
    /// Interaction logic for add_tax_filing_history.xaml
    /// </summary>
    public partial class add_tax_filing_history : UserControl, IModalClosing
    {
        public static readonly DependencyProperty IsManualProperty = DependencyProperty.Register(nameof(IsManual), typeof(bool), typeof(add_tax_filing_history));
        public bool IsManual
        {
            get { return (bool)GetValue(IsManualProperty); }
            set { SetValue(IsManualProperty, value); }
        }

        public add_tax_filing_history()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var companyList = TaxCalendarHelper.GetCompanyListAsync().GetResult().OrderBy(i => i.CompanyName).ToList();
            companyList.Insert(0, null);
            CompanyValue.ItemsSource = new ObservableCollection<TaxFilingCompanyViewModel>(companyList);
        }

        private void Payment_OpenCamera_btn_Click(object sender, RoutedEventArgs e)
        {
            payment_camera.Open();
            if (payment_camera.IsSelected)
            {
                if (payment_camera.CapturedImage != null)
                {
                    //save captureed image to temp folder
                    JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(payment_camera.CapturedImage));
                    string imgFile = Path.Combine(Path.GetTempPath(), string.Format("captured_{0}.jpg", DateTime.Now.ToString("yyyy-MM-dd-hhhmmss")));
                    using (FileStream fs = new FileStream(imgFile, FileMode.Create, FileAccess.Write))
                    {
                        encoder.Save(fs);
                    }
                    //set filename value
                    PaymentFileNameValue.SetValue(FileAttachment.FileNameProperty, imgFile);
                }
            }
        }

        public void ModalClosing(ModalClosingArgs e)
        {
            if (e.Result == ModalResult.Delete)
            {
                e.Cancel = MessageBox.Show("Are you sure to DELETE this payment?", "DELETE", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes;
            }
        }
    }
}
