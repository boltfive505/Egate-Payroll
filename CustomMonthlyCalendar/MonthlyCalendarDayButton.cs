using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace CustomMonthlyCalendar
{
    [TemplatePart(Name = "PART_DayTitleButton", Type = typeof(Button))]
    public class MonthlyCalendarDayButton : Control
    {
        private const string ELEMENT_DAYTITLEBUTTON = "PART_DayTitleButton";
        private Button _dayTitle;

        public static readonly DependencyProperty DayProperty = DependencyProperty.Register(nameof(Day), typeof(DateTime), typeof(MonthlyCalendarDayButton));
        public DateTime Day
        {
            get { return (DateTime)GetValue(DayProperty); }
            set { SetValue(DayProperty, value); }
        }

        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(nameof(Content), typeof(object), typeof(MonthlyCalendarDayButton));
        public object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        public static readonly DependencyProperty TitleExtraContentProperty = DependencyProperty.Register(nameof(TitleExtraContent), typeof(object), typeof(MonthlyCalendarDayButton));
        public object TitleExtraContent
        {
            get { return GetValue(TitleExtraContentProperty); }
            set { SetValue(TitleExtraContentProperty, value); }
        }

        public static readonly DependencyProperty IsDisplayedMonthProperty = DependencyProperty.Register(nameof(IsDisplayedMonth), typeof(bool), typeof(MonthlyCalendarDayButton));
        public bool IsDisplayedMonth
        {
            get { return (bool)GetValue(IsDisplayedMonthProperty); }
            set { SetValue(IsDisplayedMonthProperty, value); }
        }

        public static readonly DependencyProperty IsTodayProperty = DependencyProperty.Register(nameof(IsToday), typeof(bool), typeof(MonthlyCalendarDayButton));
        public bool IsToday
        {
            get { return (bool)GetValue(IsTodayProperty); }
            set { SetValue(IsTodayProperty, value); }
        }

        public event EventHandler DayClick;

        static MonthlyCalendarDayButton()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(MonthlyCalendarDayButton), new FrameworkPropertyMetadata(typeof(MonthlyCalendarDayButton)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this._dayTitle = this.GetTemplateChild(ELEMENT_DAYTITLEBUTTON) as Button;
            this._dayTitle.Click += _dayTitle_Click;
        }

        private void _dayTitle_Click(object sender, RoutedEventArgs e)
        {
            DayClick?.Invoke(this, new EventArgs());
        }
    }
}
