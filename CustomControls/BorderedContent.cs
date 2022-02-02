using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace CustomControls
{
    [ContentProperty("Content")]
    public class BorderedContent : Control
    {
        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(nameof(Content), typeof(object), typeof(BorderedContent));
        public object Content
        { 
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        public BorderedContent()
        { }

        static BorderedContent()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(BorderedContent), new FrameworkPropertyMetadata(typeof(BorderedContent)));
        }
    }
}
