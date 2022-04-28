using JobViewer.Common;
using System;
using System.Windows;
using System.Windows.Controls;

namespace JobViewer.Controls.Statistics
{
    public partial class StatisticsPLotterView : UserControl
    {
        public String Header
        {
            get { return (String)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(String), typeof(StatisticsPLotterView));

        public StatisticInfo StatisticInfo
        {
            get { return (StatisticInfo)GetValue(StatisticInfoProperty); }
            set { SetValue(StatisticInfoProperty, value); }
        }
        public static readonly DependencyProperty StatisticInfoProperty =
            DependencyProperty.Register("StatisticInfo", typeof(StatisticInfo), typeof(StatisticsPLotterView), new PropertyMetadata(new PropertyChangedCallback(OnPropertyChanged)));

        public StatisticsPLotterView()
        {
            InitializeComponent();
        }

        private static void OnPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (StatisticsPLotterView)obj;
            if (e.Property == StatisticInfoProperty)
                ctrl.PlotStatistic();
        }

        private void PlotStatistic(bool isYDate = true)
        {
            plot.Plot.Clear();

            if (StatisticInfo.DataX.Length > 0 && StatisticInfo.DataY.Length == StatisticInfo.DataY.Length)
            {
                plot.Plot.AddBar(StatisticInfo.DataY, StatisticInfo.DataX);
                if (isYDate)
                    plot.Plot.XAxis.DateTimeFormat(true);

                plot.Plot.SetAxisLimits(yMin: 0);
            }

            plot.Refresh();
        }
    }
}
