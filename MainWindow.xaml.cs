using JobViewer.Common;
using ScottPlot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace JobViewer
{
    public partial class MainWindow : Window
    {
        public int TimerInterval
        {
            get { return (int)GetValue(TimerIntervalProperty); }
            set { SetValue(TimerIntervalProperty, value); }
        }
        public static readonly DependencyProperty TimerIntervalProperty =
            DependencyProperty.Register("TimerInterval", typeof(int), typeof(MainWindow), new PropertyMetadata(60));

        public string TimerLastTime
        {
            get { return (string)GetValue(TimerLastTimeProperty); }
            set { SetValue(TimerLastTimeProperty, value); }
        }
        public static readonly DependencyProperty TimerLastTimeProperty =
            DependencyProperty.Register("TimerLastTime", typeof(string), typeof(MainWindow), new PropertyMetadata(string.Empty));


        private readonly MainViewModel dataContext;
        public MainWindow()
        {
            InitializeComponent();

            if (!ShowConnectionWindow())
                Application.Current.Shutdown();

            DataContext = dataContext = new MainViewModel();
            dataContext.PropertyChanged += DataContextPropertyChanged;
        }

        private void DataContextPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MainViewModel.StepStatistic))
                PlotStatistic(plotStep, dataContext.StepStatistic);

            else if (e.PropertyName == nameof(MainViewModel.JobStatistic))
                PlotStatistic(plotJob, dataContext.JobStatistic);

            else if (e.PropertyName == nameof(MainViewModel.JobStepAvgStatistic))
                PlotStatistic(plotJobStepAvg, dataContext.JobStepAvgStatistic, false);
            
        }


        private bool ShowConnectionWindow()
        {
            var connection = new ConnectionWindow();
            connection.ShowDialog();
            return connection.DialogResult ?? false;
        }

        private void PlotStatistic(WpfPlot plot, StatisticInfo info, bool isYDate = true)
        {
            plot.Plot.Clear();
            if (info.DataX.Length > 0 && info.DataY.Length == info.DataY.Length)
            {
                plot.Plot.AddBar(info.DataY, info.DataX);
                if (isYDate)
                    plot.Plot.XAxis.DateTimeFormat(true);

                plot.Plot.SetAxisLimits(yMin: 0);
            }

            plot.Refresh();
        }

        DispatcherTimer refreshDataTimer = new DispatcherTimer();
        private void AutoRefreshChecked(object sender, RoutedEventArgs e)
        {
            refreshDataTimer.Interval = TimeSpan.FromSeconds(TimerInterval);
            refreshDataTimer.Tick += RefreshDataTimer_Tick;
            refreshDataTimer.Start();
        }

        private void RefreshDataTimer_Tick(object sender, EventArgs e)
        {
            TimerLastTime = $"refreshed at {DateTime.Now.ToLongTimeString()}";
            dataContext.LoadJobHistory();
        }

        private void AutoRefresh_Unchecked(object sender, RoutedEventArgs e)
        {
            refreshDataTimer.Stop();
            refreshDataTimer.Tick -= RefreshDataTimer_Tick;
            TimerLastTime = string.Empty;
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            ShowSettings();
        }

        private void ShowSettings()
        {
            new SettingsWindow().ShowDialog();
        }
    }
}
