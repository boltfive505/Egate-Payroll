using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CustomControls.Modal.Primitive
{
    public class ModalButton : Button
    {
        public static readonly DependencyProperty ResultCommandProperty = DependencyProperty.Register(nameof(ResultCommand), typeof(ResultCommand), typeof(ModalButton));
        public ResultCommand ResultCommand
        {
            get { return (ResultCommand)GetValue(ResultCommandProperty); }
            set { SetValue(ResultCommandProperty, value); }
        }

        public static readonly DependencyProperty ResultParameterProperty = DependencyProperty.Register(nameof(ResultParameter), typeof(ModalResult), typeof(ModalButton), new FrameworkPropertyMetadata(ModalResult.Ok));
        public ModalResult ResultParameter
        {
            get { return (ModalResult)GetValue(ResultParameterProperty); }
            set { SetValue(ResultParameterProperty, value); }
        }

        public static readonly DependencyProperty ResultKeyProperty = DependencyProperty.Register(nameof(ResultKey), typeof(object), typeof(ModalButton));
        public object ResultKey
        {
            get { return (object)GetValue(ResultKeyProperty); }
            set { SetValue(ResultKeyProperty, value); }
        }

        public ModalButton()
        {
            this.Click += ModalButton_Click;
        }

        static ModalButton()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(ModalButton), new FrameworkPropertyMetadata(typeof(ModalButton)));
        }

        private void ModalButton_Click(object sender, RoutedEventArgs e)
        {
            if (ResultCommand != null)
                ResultCommand.Execute(ResultParameter, ResultKey);
        }
    }
}
