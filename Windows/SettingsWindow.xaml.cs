using System.Windows;

namespace JobViewer
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Save();
            this.DialogResult = true;
        }
    }
}
