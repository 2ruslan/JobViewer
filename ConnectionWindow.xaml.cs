using System.Text;
using System.Windows;

namespace JobViewer
{
    public partial class ConnectionWindow : Window
    {
        public ConnectionWindow()
        {
            InitializeComponent();

            WindowsAuthenticationCtrl.IsChecked = Properties.Settings.Default.UseWindowsAutentification;
            LoginAuthenticationCtrl.IsChecked = !WindowsAuthenticationCtrl.IsChecked;

        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.UseWindowsAutentification = WindowsAuthenticationCtrl.IsChecked??false;
            Properties.Settings.Default.Save();

            this.DialogResult = true;
        }

        public string GetConnectionString()
        {
            var prop = Properties.Settings.Default;

            var connectionString = new StringBuilder($"data source={prop.DataSource};initial catalog = msdb;");

            if (prop.UseWindowsAutentification)
                connectionString.Append("Integrated Security = SSPI;");
            else
                connectionString.Append($"User ID={prop.User};Password={PasswordBox.Password};");

            return connectionString.ToString();
        }
    }
}
