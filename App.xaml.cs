using JobViewer.Common;
using JobViewer.Messages;
using JobViewer.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace JobViewer
{
    public partial class App : Application
    {

        public App()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

          
        }
        public ApplicationContext ApplicationContext { get; private set; }

        private void ApplicationStart(object sender, StartupEventArgs e)
        {
            var connection = GetDbConnection();
            if (connection == null)
            {
                Application.Current.Shutdown(-1);
                return;
            }

            ApplicationContext = new ApplicationContext(new Messanger(), connection);

            ShowMainWindow();
        }
        
        private void ShowMainWindow()
        {
            var main = new MainWindow();
            Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
            Current.MainWindow = main;
            main.ShowDialog();
        }

        private JobDbContext GetDbConnection()
        {
            Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
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


        private void OnDbConnected(JobDbContext connection)
        {
            ApplicationContext = new ApplicationContext(new Messanger(), GetDbConnection());
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            
        }

    }
}
