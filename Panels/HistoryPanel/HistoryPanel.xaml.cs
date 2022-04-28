using JobViewer.Base;
using JobViewer.Messages;
using System;
using System.Windows;
using System.Windows.Threading;

namespace JobViewer.Panels.HistoryPanel
{
    public partial class HistoryPanel : BaseUserControl
    {
        private HistoryPanelViewModel viewModel;

        public int TimerInterval
        {
            get { return (int)GetValue(TimerIntervalProperty); }
            set { SetValue(TimerIntervalProperty, value); }
        }
        public static readonly DependencyProperty TimerIntervalProperty =
            DependencyProperty.Register("TimerInterval", typeof(int), typeof(HistoryPanel), new PropertyMetadata(60));

        public string TimerLastTime
        {
            get { return (string)GetValue(TimerLastTimeProperty); }
            set { SetValue(TimerLastTimeProperty, value); }
        }
        public static readonly DependencyProperty TimerLastTimeProperty =
            DependencyProperty.Register("TimerLastTime", typeof(string), typeof(HistoryPanel), new PropertyMetadata(string.Empty));

        DispatcherTimer refreshDataTimer = new DispatcherTimer();

        public HistoryPanel()
        {
            InitializeComponent();
            
            DataContext = viewModel = new HistoryPanelViewModel();

            ApplicationContext?.Messanger
                .Subscribe(typeof(MessageStepChanged), OnRecieveMessage);
        }

        private void OnRecieveMessage(IMessage m)
        {
            if (m is MessageStepChanged message)
                viewModel.LoadJobHistory(ApplicationContext.JobDbContext, message.NewStep);
        }

        private void AutoRefreshChecked(object sender, RoutedEventArgs e)
        {
            refreshDataTimer.Interval = TimeSpan.FromSeconds(TimerInterval);
            refreshDataTimer.Tick += RefreshDataTimerTick;
            refreshDataTimer.Start();
            TimerLastTime = $"started at {DateTime.Now.ToLongTimeString()}";
        }

        private void RefreshDataTimerTick(object sender, EventArgs e)
        {
            TimerLastTime = $"updated at {DateTime.Now.ToLongTimeString()}";
            viewModel.LoadLastJobHistory();
        }

        private void AutoRefreshUnchecked(object sender, RoutedEventArgs e)
        {
            refreshDataTimer.Stop();
            refreshDataTimer.Tick -= RefreshDataTimerTick;
            TimerLastTime = string.Empty;
        }
    }
}
