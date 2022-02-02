using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace CustomControls.Modal.Primitive
{
    [TemplatePart(Name = "PART_Close", Type = typeof(Button))]
    [TemplatePart(Name = "PART_VisualBrush", Type = typeof(VisualBrush))]
    [TemplatePart(Name = "PART_ModalMask", Type = typeof(Border))]
    public class ModalItem : Control
    {
        private const string ELEMENT_CLOSEBTN = "PART_Close";
        private const string ELEMENT_VISUALBRUSH = "PART_VisualBrush";
        private const string ELEMENT_MODALMASK = "PART_ModalMask";

        private Button _closeBtn;
        private VisualBrush _visualBrush;
        private Border _modalMask;

        private DispatcherFrame _frame;
        private IModalClosed _modalClosed;
        private IModalClosing _modalClosing;
        private IModalCommand _modalCommand;

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string), typeof(ModalItem));
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(nameof(Content), typeof(object), typeof(ModalItem));
        public object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        public static readonly DependencyProperty ButtonsTemplateProperty = DependencyProperty.Register(nameof(ButtonsTemplate), typeof(DataTemplate), typeof(ModalItem));
        public DataTemplate ButtonsTemplate
        { 
            get { return (DataTemplate)GetValue(ButtonsTemplateProperty); }
            set { SetValue(ButtonsTemplateProperty, value); }
        }

        private ModalResult _result = ModalResult.Ok;
        public ModalResult Result { get { return _result; } }

        private object _key;
        public object Key { get { return _key; } }

        private ResultCommand _resultCommand;
        public ResultCommand ResultCommand
        {
            get
            {
                if (_resultCommand == null)
                {
                    _resultCommand = new ResultCommand(CloseWithResult);
                }
                return _resultCommand;
            }
        }

        public ModalItem()
        { }

        static ModalItem()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(ModalItem), new FrameworkPropertyMetadata(typeof(ModalItem)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this._closeBtn = this.GetTemplateChild(ELEMENT_CLOSEBTN) as Button;
            this._visualBrush = this.GetTemplateChild(ELEMENT_VISUALBRUSH) as VisualBrush;
            this._modalMask = this.GetTemplateChild(ELEMENT_MODALMASK) as Border;

            UpdateTemplate();
        }

        private void UpdateTemplate()
        {
            _visualBrush.Visual = _modalMask;
            _closeBtn.Click += (sender, e) => CloseWithResult(ModalResult.Ok, null);
        }

        internal void DoShowModal(object content, string title)
        {
            this.Content = content;
            this.Title = title;

            _modalClosed = content as IModalClosed;
            _modalClosing = content as IModalClosing;
            _modalCommand = content as IModalCommand;

            if (_modalCommand != null)
                _modalCommand.ExecuteResult = CloseWithResult;

            Show();
            if (_modalClosed != null)
                _modalClosed.ModalClosed(new ModalClosedArgs(_result, _key));

            //clear reference to modal events
            _modalClosed = null;
            _modalClosing = null;
            _modalCommand = null;
        }

        private void Show()
        {
            _frame = new DispatcherFrame();
            Dispatcher.PushFrame(_frame);
        }

        private void Close()
        {
            _frame.Continue = false;
        }

        private void CloseWithResult(ModalResult result, object key)
        {
            if (_modalClosing != null)
            {
                ModalClosingArgs closing = new ModalClosingArgs(result, key);
                _modalClosing.ModalClosing(closing);
                if (closing.Cancel)
                    return;
            }

            Close();
            _result = result;
            _key = key;
        }
    }
}
