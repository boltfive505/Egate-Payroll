using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections.Specialized;
using Microsoft.Win32;
using System.IO;

namespace CustomControls
{
    [TemplatePart(Name = "PART_BrowseFile", Type = typeof(Button))]
    [TemplatePart(Name = "PART_RemoveFile", Type = typeof(Button))]
    public class FileAttachment : Control
    {
        private const string ELEMENT_BROWSEFILE = "PART_BrowseFile";
        private const string ELEMENT_REMOVEFILE = "PART_RemoveFile";

        private Button _browseFile;
        private Button _removeFile;

        public static readonly DependencyProperty FileNameProperty = DependencyProperty.Register(nameof(FileName), typeof(string), typeof(FileAttachment));
        public string FileName
        {
            get { return (string)GetValue(FileNameProperty); }
            set { SetValue(FileNameProperty, value); }
        }

        public static readonly DependencyProperty FilterProperty = DependencyProperty.Register(nameof(Filter), typeof(string), typeof(FileAttachment));
        public string Filter
        {
            get { return (string)GetValue(FilterProperty); }
            set { SetValue(FilterProperty, value); }
        }

        public static readonly DependencyProperty CanRemoveFileProperty = DependencyProperty.Register(nameof(CanRemoveFile), typeof(bool), typeof(FileAttachment), new PropertyMetadata(false));
        public bool CanRemoveFile
        {
            get { return (bool)GetValue(CanRemoveFileProperty); }
            set { SetValue(CanRemoveFileProperty, value); }
        }

        public FileAttachment()
        {
            
        }

        static FileAttachment()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(FileAttachment), new FrameworkPropertyMetadata(typeof(FileAttachment)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this._browseFile = this.GetTemplateChild(ELEMENT_BROWSEFILE) as Button;
            this._removeFile = this.GetTemplateChild(ELEMENT_REMOVEFILE) as Button;
            UpdateTemplate();
        }

        private void UpdateTemplate()
        {
            _browseFile.Click += _browseFile_Click;
            _removeFile.Click += _removeFile_Click;
        }

        private void _removeFile_Click(object sender, RoutedEventArgs e)
        {
            this.SetValue(FileNameProperty, null);
        }

        private void _browseFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Title = "Select File";
            open.Multiselect = false;
            open.Filter = string.IsNullOrWhiteSpace(this.Filter) ? "Any File|*.*" : this.Filter;
            if (open.ShowDialog() == true)
            {
                this.SetValue(FileNameProperty, open.FileName);
            }
        }
    }
}
