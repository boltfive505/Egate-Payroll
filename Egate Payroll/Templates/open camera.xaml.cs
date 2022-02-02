using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using AForge.Video;
using AForge.Video.DirectShow;

namespace Egate_Payroll.Templates
{
    /// <summary>
    /// Interaction logic for open_camera.xaml
    /// </summary>
    public partial class open_camera : UserControl
    {
        public static readonly DependencyProperty CapturedImageProperty = DependencyProperty.Register(nameof(CapturedImage), typeof(BitmapImage), typeof(open_camera));
        public BitmapImage CapturedImage
        {
            get { return (BitmapImage)GetValue(CapturedImageProperty); }
            set { SetValue(CapturedImageProperty, value); }
        }

        public bool IsSelected { get; set; }

        private DispatcherFrame _frame;
        private bool captureSnapshot = false;

        public open_camera()
        {
            InitializeComponent();

            if (DesignerProperties.GetIsInDesignMode(this)) return;
            cameraList_cbox.ItemsSource = GetCameraDeviceList();
            cameraList_cbox.SelectedIndex = 0;
        }

        private IEnumerable<FilterInfo> GetCameraDeviceList()
        {
            var devices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            return devices.OfType<FilterInfo>();
        }

        private void cameraList_cbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this)) return;
            var info = (sender as ComboBox).SelectedItem as FilterInfo;
            if (info != null)
            {
                var device = new VideoCaptureDevice(info.MonikerString);
                OpenVideoSource(device);
            }
        }

        private void OpenVideoSource(IVideoSource source)
        {
            CloseCurrentVideoSource();
            videoSourcePlayer.VideoSource = source;
            videoSourcePlayer.Start();
        }

        private void CloseCurrentVideoSource()
        {
            if (videoSourcePlayer.VideoSource != null)
            {
                videoSourcePlayer.SignalToStop();
                if (videoSourcePlayer.IsRunning)
                    videoSourcePlayer.Stop();
                videoSourcePlayer.VideoSource = null;
            }
        }

        //runs on separate thread
        private void videoSourcePlayer_NewFrame(object sender, ref System.Drawing.Bitmap image)
        {
            Size actualSize = new Size(image.Width, image.Height);
            Dispatcher.Invoke(() => DoResizeVideoSourcePlayer(actualSize));

            if (captureSnapshot)
            {
                var img = image;
                Dispatcher.Invoke(() => DoCaptureSnapshot(img));
                captureSnapshot = false;
            }
        }

        private void DoResizeVideoSourcePlayer(Size actualSize)
        {
            Size maxSize = new Size(cameraContainer.ActualWidth, cameraContainer.ActualHeight);
            Size resize = Classes.ImageHelper.Resize(actualSize, maxSize);
            cameraHost.Width = resize.Width;
            cameraHost.Height = resize.Height;
        }

        private void DoCaptureSnapshot(System.Drawing.Bitmap image)
        {
            BitmapImage bitmap = new BitmapImage();
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                ms.Position = 0;
                bitmap.BeginInit();
                bitmap.StreamSource = ms;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
            }
            bitmap.Freeze();
            CapturedImage = bitmap;
            cameraContainer.Visibility = Visibility.Collapsed;
            snapshotContainer.Visibility = Visibility.Visible;
            capture_btn.IsEnabled = false;
            retry_btn.IsEnabled = true;
        }

        public void RetryCapture()
        {
            CapturedImage = null;
            cameraContainer.Visibility = Visibility.Visible;
            snapshotContainer.Visibility = Visibility.Collapsed;
            capture_btn.IsEnabled = true;
            retry_btn.IsEnabled = false;
        }

        private void capture_btn_Click(object sender, RoutedEventArgs e)
        {
            captureSnapshot = true;
        }

        private void retry_btn_Click(object sender, RoutedEventArgs e)
        {
            RetryCapture();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            CloseCurrentVideoSource();
        }

        public void Open()
        {
            RetryCapture();
            this.Visibility = Visibility.Visible;
            _frame = new DispatcherFrame();
            Dispatcher.PushFrame(_frame);
        }

        public void Close()
        {
            _frame.Continue = false;
            this.Visibility = Visibility.Collapsed;
        }

        private void Select_btn_Click(object sender, RoutedEventArgs e)
        {
            IsSelected = true;
            Close();
        }

        private void Close_btn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
