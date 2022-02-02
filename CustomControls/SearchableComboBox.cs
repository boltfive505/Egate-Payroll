using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CustomControls
{
    [TemplatePart(Name = "PART_DropDownFilterTextBox", Type = typeof(TextBox))]
    [TemplatePart(Name = "PART_ItemsPresenter", Type = typeof(ItemsPresenter))]
    public class SearchableComboBox : ComboBox
    {
        private const string ELEMENT_DROPDOWWNFILTERTEXTBOX = "PART_DropDownFilterTextBox";
        private const string ELEMENT_ITEMSPRESENTER = "PART_ItemsPresenter";

        private TextBox _filterTextbox;
        private ItemsPresenter _itemsPresenter;
        private object _selectedValue;

        public delegate bool SearchTextCallback(object item, string searchText);
        public event SearchTextCallback SearchText;

        public SearchableComboBox()
        {
            DropDownOpened += SearchableComboBox_DropDownOpened;
            DropDownClosed += SearchableComboBox_DropDownClosed;
        }

        static SearchableComboBox()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(SearchableComboBox), new FrameworkPropertyMetadata(typeof(SearchableComboBox)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _filterTextbox = this.GetTemplateChild(ELEMENT_DROPDOWWNFILTERTEXTBOX) as TextBox;
            _itemsPresenter = this.GetTemplateChild(ELEMENT_ITEMSPRESENTER) as ItemsPresenter;
            UpdateTemplate();
        }

        private void UpdateTemplate()
        {
            _filterTextbox.TextChanged += _filterTextbox_TextChanged;
            _filterTextbox.KeyDown += _filterTextbox_KeyDown;
            _filterTextbox.GotMouseCapture += _filterTextbox_GotMouseCapture;
        }

        private void _filterTextbox_GotMouseCapture(object sender, MouseEventArgs e)
        {
            TextBox textBox = ((TextBox)sender);
            textBox.Focus();
            e.Handled = true;
        }

        private void _filterTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Items.Filter += x => DoFilterItem(x);
        }

        private void _filterTextbox_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    _filterTextbox.Text = string.Empty;
                    this.SelectedValue = _selectedValue;
                    break;
                case Key.Down:
                    _itemsPresenter.Focus();
                    break;
            }
        }

        private bool DoFilterItem(object obj)
        {
            string search = _filterTextbox.Text.Trim();
            if (SearchText != null)
            {
                return SearchText(obj, search);
            }
            else
            {
                string str = System.Convert.ToString(obj);
                return str.IndexOf(search, 0, StringComparison.InvariantCultureIgnoreCase) >= 0;
            }
        }

        private void SearchableComboBox_DropDownOpened(object sender, EventArgs e)
        {
            ComboBox cbo = (ComboBox)sender;
            if (cbo.SelectedValue != null)
            {
                _selectedValue = cbo.SelectedValue;
            }
        }

        private void SearchableComboBox_DropDownClosed(object sender, EventArgs e)
        {
            ComboBox cbo = (ComboBox)sender;
            _filterTextbox.Text = string.Empty;
            _selectedValue = null;
        }
    }
}
