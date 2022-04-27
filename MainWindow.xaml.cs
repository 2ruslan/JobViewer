using JobViewer.Common;
using JobViewer.Model;
using ScottPlot;
using System;
using System.ComponentModel;
using System.Windows;
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

            var connection = GetDbConnection();

            if (connection == null)
                Application.Current.Shutdown();

            DataContext = dataContext = new MainViewModel(connection);

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

        private JobDbContext GetDbConnection()
        {
            var dlg = new ConnectionWindow();

            dlg.ShowDialog();
            if (!(dlg.DialogResult ?? false))
                return null;
            
            var connection = new JobDbContext(dlg.GetConnectionString());

            if (!connection.Database.CanConnect())
            {
                MessageBox.Show("Error connection!");
                return GetDbConnection();
            }

            return connection;
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
