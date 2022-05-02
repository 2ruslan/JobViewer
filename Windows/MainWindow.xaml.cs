using JobViewer.Common;
using JobViewer.Messages;
using MahApps.Metro.Controls;
using System.Windows;

namespace JobViewer
{
    public partial class MainWindow : MetroWindow
    {
        protected ApplicationContext ApplicationContext
        {
            get
            {
                if (App.Current != null && App.Current is App app)
                    return app.ApplicationContext;
                return null;
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            ApplicationContext.Messanger.Send(new MessageJobsLoad());
        }

        private void SettingsClick(object sender, RoutedEventArgs e)
        {
            ShowSettings();
        }

        private void ShowSettings()
        {
            new SettingsWindow().ShowDialog();
        }
    }
}
