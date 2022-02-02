using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections.Specialized;

namespace CustomMonthlyCalendar
{
    [TemplatePart(Name = "PART_DaysOfWeek", Type = typeof(Grid))]
    [TemplatePart(Name = "PART_DaysOfMonth", Type = typeof(Grid))]
    [TemplatePart(Name = "PART_PreviousMonth", Type = typeof(Button))]
    [TemplatePart(Name = "PART_NextMonth", Type = typeof(Button))]
    [TemplatePart(Name = "PART_TodayMonth", Type = typeof(Button))]
    public class MonthlyCalendar : Control
    {
        private const string ELEMENT_DAYSOFWEEK = "PART_DaysOfWeek";
        private const string ELEMENT_DAYSOFMONTH = "PART_DaysOfMonth";
        private const string ELEMENT_PREVIOUSMONTH = "PART_PreviousMonth";
        private const string ELEMENT_NEXTMONTH = "PART_NextMonth";
        private const string ELEMENT_TODAY = "PART_TodayMonth";
        private const string KEY_DAYSOFWEEK_TITLE_TEMPLATE = "DaysOfWeekTitleTemplate";
        public const int DAY_COUNT_IN_FULL_MONTH = 42;
        private static ComponentResourceKey _daysOfWeekTitleTemplateResourceKey;

        private Grid _daysOfWeekView;
        private Grid _daysOfMonthView;
        private DataTemplate _daysOfWeekTemplate;
        private Button _previousMonth;
        private Button _nextMonth;
        private Button _todayMonth;
        private MonthlyCalendarDayButton[] dayBtns;

        public static ComponentResourceKey DaysOfWeekTitleTemplateResourceKey
        {
            get
            {
                if (MonthlyCalendar._daysOfWeekTitleTemplateResourceKey == null)
                    MonthlyCalendar._daysOfWeekTitleTemplateResourceKey = new ComponentResourceKey(typeof(MonthlyCalendar), KEY_DAYSOFWEEK_TITLE_TEMPLATE);
                return MonthlyCalendar._daysOfWeekTitleTemplateResourceKey;
            }
        }

        public event EventHandler DisplayMonthChanged;

        public static readonly DependencyProperty MonthlyCalendarDayButtonStyleProperty = DependencyProperty.Register(nameof(MonthlyCalendarDayButtonStyle), typeof(Style), typeof(MonthlyCalendar));
        public Style MonthlyCalendarDayButtonStyle
        {
            get { return (Style)GetValue(MonthlyCalendarDayButtonStyleProperty); }
            set { SetValue(MonthlyCalendarDayButtonStyleProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(MonthlyCalendar), new PropertyMetadata(new PropertyChangedCallback(OnItemsSourcePropertyChanged)));
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty BindingTemplateProperty = DependencyProperty.Register(nameof(BindingTemplate), typeof(DataTemplate), typeof(MonthlyCalendar));
        public DataTemplate BindingTemplate
        {
            get { return (DataTemplate)GetValue(BindingTemplateProperty); }
            set { SetValue(BindingTemplateProperty, value); }
        }

        public static readonly DependencyProperty EmptyTemplateProperty = DependencyProperty.Register(nameof(EmptyTemplate), typeof(DataTemplate), typeof(MonthlyCalendar));
        public DataTemplate EmptyTemplate
        {
            get { return (DataTemplate)GetValue(EmptyTemplateProperty); }
            set { SetValue(EmptyTemplateProperty, value); }
        }

        public static readonly DependencyProperty TitleExtraTemplateProperty = DependencyProperty.Register(nameof(TitleExtraTemplate), typeof(DataTemplate), typeof(MonthlyCalendar));
        public DataTemplate TitleExtraTemplate
        {
            get { return (DataTemplate)GetValue(TitleExtraTemplateProperty); }
            set { SetValue(TitleExtraTemplateProperty, value); }
        }

        public static readonly DependencyProperty DisplayMonthProperty = DependencyProperty.Register(nameof(DisplayMonth), typeof(DateTime), typeof(MonthlyCalendar), new PropertyMetadata(new PropertyChangedCallback(OnDisplayMonthChanged)));
        public DateTime DisplayMonth
        {
            get { return (DateTime)GetValue(DisplayMonthProperty); }
            set { SetValue(DisplayMonthProperty, value); }
        }

        public static readonly RoutedEvent DayClickEvent = EventManager.RegisterRoutedEvent(nameof(DayClick), RoutingStrategy.Bubble, typeof(DayClickEventHandler), typeof(MonthlyCalendar));
        public event DayClickEventHandler DayClick
        {
            add { AddHandler(DayClickEvent, value); }
            remove { RemoveHandler(DayClickEvent, value); }
        }

        public DateTime DisplayDateStart { get; internal set; }
        public DateTime DisplayDateEnd { get; internal set; }

        public MonthlyCalendar()
        {
            this.DisplayMonth = DateTime.Now;
        }

        static MonthlyCalendar()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(MonthlyCalendar), new FrameworkPropertyMetadata(typeof(MonthlyCalendar)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this._daysOfWeekView = this.GetTemplateChild(ELEMENT_DAYSOFWEEK) as Grid;
            this._daysOfMonthView = this.GetTemplateChild(ELEMENT_DAYSOFMONTH) as Grid;
            if (this.Template != null && this.Template.Resources.Contains(MonthlyCalendar.DaysOfWeekTitleTemplateResourceKey))
                this._daysOfWeekTemplate = this.Template.Resources[MonthlyCalendar.DaysOfWeekTitleTemplateResourceKey] as DataTemplate;
            this._previousMonth = this.GetTemplateChild(ELEMENT_PREVIOUSMONTH) as Button;
            this._nextMonth = this.GetTemplateChild(ELEMENT_NEXTMONTH) as Button;
            this._todayMonth = this.GetTemplateChild(ELEMENT_TODAY) as Button;
            UpdateTemplate();
        }

        private void UpdateTemplate()
        {
            UpdateCalendarHeader();
            UpdateDaysOfWeekView();
            UpdateDaysOfMonthView();
            UpdateDaysButtonDay();
            UpdateBindingTemplates();
        }

        private void UpdateCalendarHeader()
        {
            this._previousMonth.Click += _previousMonth_Click;
            this._nextMonth.Click += _nextMonth_Click;
            this._todayMonth.Click += _todayMonth_Click;
        }

        private void _nextMonth_Click(object sender, RoutedEventArgs e)
        {
            SetDisplayMonth(1);
        }

        private void _previousMonth_Click(object sender, RoutedEventArgs e)
        {
            SetDisplayMonth(-1);
        }

        private void _todayMonth_Click(object sender, RoutedEventArgs e)
        {
            DisplayMonth = DateTime.Now;
        }

        private void SetDisplayMonth(int offset)
        {
            DisplayMonth = DisplayMonth.AddMonths(offset);
        }

        private void UpdateDaysOfWeekView()
        {
            DayOfWeek[] weeksArr = (DayOfWeek[])Enum.GetValues(typeof(DayOfWeek));
            for (int i = 0; i < weeksArr.Length; i++)
            {
                FrameworkElement template = this._daysOfWeekTemplate != null ? (FrameworkElement)this._daysOfWeekTemplate.LoadContent() : (FrameworkElement)new ContentControl();
                Grid.SetColumn(template, i);
                template.DataContext = weeksArr[i];
                _daysOfWeekView.Children.Add(template);
            }
        }

        private void UpdateDaysOfMonthView()
        {
            dayBtns = new MonthlyCalendarDayButton[DAY_COUNT_IN_FULL_MONTH];
            //7 days, 6 weeks
            int i = 0;
            for (int r = 0; r < 6; r++)
            {
                for (int c = 0; c < 7; c++)
                {
                    MonthlyCalendarDayButton btn = new MonthlyCalendarDayButton();
                    btn.DayClick += Btn_DayClick;
                    Grid.SetColumn(btn, c);
                    Grid.SetRow(btn, r);
                    btn.SetBinding(FrameworkElement.StyleProperty, new Binding(nameof(MonthlyCalendarDayButtonStyle)) { Source = this });
                    this._daysOfMonthView.Children.Add(btn);
                    dayBtns[i] = btn;
                    i++;
                }
            }
        }

        private void Btn_DayClick(object sender, EventArgs e)
        {
            MonthlyCalendarDayButton btn = sender as MonthlyCalendarDayButton;
            DateTime day = btn.Day;
            RaiseEvent(new DayClickEventArgs(DayClickEvent, btn, day));
        }

        private void UpdateDaysButtonDay()
        {
            if (dayBtns == null || dayBtns.Length == 0) return;
            DateTime today = DateTime.Now.Date;
            int btnIndex = 0;
            //get days from last week of the previous month
            DateTime previousMonth = DisplayMonth.AddMonths(-1);
            DateTime lastDayOfPreviousMonth = new DateTime(previousMonth.Year, previousMonth.Month, DateTime.DaysInMonth(previousMonth.Year, previousMonth.Month));
            int daysCountOfLastWeekFromLastMonth = (int)lastDayOfPreviousMonth.DayOfWeek + 1;
            for (int i = 0; i < daysCountOfLastWeekFromLastMonth; i++)
            {
                DateTime day = new DateTime(previousMonth.Year, previousMonth.Month, lastDayOfPreviousMonth.Day - (daysCountOfLastWeekFromLastMonth - i - 1));
                dayBtns[btnIndex].Day = day;
                dayBtns[btnIndex].IsDisplayedMonth = false;
                dayBtns[btnIndex].IsToday = day.Date == today;
                btnIndex++;
            }

            //get days from current display month
            int dayCount = DateTime.DaysInMonth(DisplayMonth.Year, DisplayMonth.Month);
            for (int i = 0; i < dayCount; i++)
            {
                DateTime day = new DateTime(DisplayMonth.Year, DisplayMonth.Month, i + 1);
                dayBtns[btnIndex].Day = day;
                dayBtns[btnIndex].IsDisplayedMonth = true;
                dayBtns[btnIndex].IsToday = day.Date == today;
                btnIndex++;
            }

            //fill remaining days from next month
            int remainingDaysToFill = DAY_COUNT_IN_FULL_MONTH - btnIndex;
            DateTime nextMonth = DisplayMonth.AddMonths(1);
            for (int i = 0; i < remainingDaysToFill; i++)
            {
                DateTime day = new DateTime(nextMonth.Year, nextMonth.Month, i + 1);
                dayBtns[btnIndex].Day = day;
                dayBtns[btnIndex].IsDisplayedMonth = false;
                dayBtns[btnIndex].IsToday = day.Date == today;
                btnIndex++;
            }

            //set display start and end dates
            DisplayDateStart = dayBtns.First().Day;
            DisplayDateEnd = dayBtns.Last().Day;
        }

        private void UpdateBindingTemplates()
        {
            if (dayBtns == null || dayBtns.Length == 0) return;
            DateTime firstDisplayDate = dayBtns.First().Day.Date;
            DateTime lastDisplayDate = dayBtns.Last().Day.Date;
            if (ItemsSource != null)
            {
                IEnumerable<IMonthlyCalendarDayItem> list = ItemsSource.Cast<IMonthlyCalendarDayItem>();
                var displayList = list.Where(i => i.Day.Date >= firstDisplayDate && i.Day.Date <= lastDisplayDate);
                foreach (var btn in dayBtns)
                {
                    FrameworkElement template = BindingTemplate != null ? (FrameworkElement)BindingTemplate.LoadContent() : (FrameworkElement)new ContentControl();
                    FrameworkElement titleTemplate = TitleExtraTemplate != null ? (FrameworkElement)TitleExtraTemplate.LoadContent() : (FrameworkElement)new ContentControl();
                    btn.Content = template;
                    btn.TitleExtraContent = titleTemplate;

                    btn.DataContext = null; //clear button datacontext
                    IMonthlyCalendarDayItem item = list.FirstOrDefault(i => i.Day.Date == btn.Day.Date);
                    if (item != null)
                    {
                        btn.DataContext = item;
                    }
                }
            }
        }

        private static void OnDisplayMonthChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var control = sender as MonthlyCalendar;
            if (control != null)
                control.OnDisplayMonthChanged();
        }

        protected virtual void OnDisplayMonthChanged()
        {
            UpdateDaysButtonDay();
            UpdateBindingTemplates();
            DisplayMonthChanged?.Invoke(this, new EventArgs());
        }

        private static void OnItemsSourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var control = sender as MonthlyCalendar;
            if (control != null)
                control.OnItemsSourceChanged((IEnumerable)e.OldValue, (IEnumerable)e.NewValue);
        }

        private void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            // Remove handler for oldValue.CollectionChanged
            var oldValueINotifyCollectionChanged = oldValue as INotifyCollectionChanged;

            if (null != oldValueINotifyCollectionChanged)
            {
                oldValueINotifyCollectionChanged.CollectionChanged -= new NotifyCollectionChangedEventHandler(ItemsSource_CollectionChanged);
            }
            // Add handler for newValue.CollectionChanged (if possible)
            var newValueINotifyCollectionChanged = newValue as INotifyCollectionChanged;
            if (null != newValueINotifyCollectionChanged)
            {
                newValueINotifyCollectionChanged.CollectionChanged += new NotifyCollectionChangedEventHandler(ItemsSource_CollectionChanged);
            }
        }

        private void ItemsSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateBindingTemplates();
        }
    }
}
