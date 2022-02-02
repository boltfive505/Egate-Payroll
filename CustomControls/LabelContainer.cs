using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace CustomControls
{
    [ContentProperty("Content")]
    public class LabelContainer : Control
    {
        public static readonly DependencyProperty PlacementProperty = DependencyProperty.Register(nameof(Placement), typeof(LabelPlacement), typeof(LabelContainer), new PropertyMetadata(LabelPlacement.Top));
        public LabelPlacement Placement
        {
            get { return (LabelPlacement)GetValue(PlacementProperty); }
            set { SetValue(PlacementProperty, value); }
        }

        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(nameof(Header), typeof(object), typeof(LabelContainer));
        public object Header
        {
            get { return (object)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(nameof(Content), typeof(object), typeof(LabelContainer));
        public object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        public static readonly DependencyProperty ContentSpacingProperty = DependencyProperty.Register(nameof(ContentSpacing), typeof(Thickness), typeof(LabelContainer));
        public Thickness ContentSpacing
        {
            get { return (Thickness)GetValue(ContentSpacingProperty); }
            set { SetValue(ContentSpacingProperty, value); }
        }

        public LabelContainer()
        { }

        static LabelContainer()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(LabelContainer), new FrameworkPropertyMetadata(typeof(LabelContainer)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        //protected override void OnGotFocus(RoutedEventArgs e)
        //{
        //    base.OnGotFocus(e);
        //    MoveFocus(new System.Windows.Input.TraversalRequest(System.Windows.Input.FocusNavigationDirection.Down));
        //}
    }
}
