using JobViewer.Base;
using JobViewer.Messages;
using System;
using System.Collections.Generic;
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

namespace JobViewer.Panels.StatistticsPanel 
{
    public partial class StatistticsPanel : BaseUserControl
    {
        private StatistticsPanelViewModel viewModel;

        public StatistticsPanel()
        {
            InitializeComponent();

            DataContext = viewModel = new StatistticsPanelViewModel();

            ApplicationContext?.Messanger
                .Subscribe(typeof(MessageJobChanged), OnRecieveMessage);
            ApplicationContext?.Messanger
                .Subscribe(typeof(MessageStepChanged), OnRecieveMessage);
        }

        private void OnRecieveMessage(IMessage m)
        {
            if (m is MessageJobChanged msgJobCh)
                viewModel.CreateJobStatistic(ApplicationContext.JobDbContext, msgJobCh.NewJob.JobId);
            else if (m is MessageStepChanged msgStpCh && msgStpCh.NewStep != null)
                viewModel.CreateStepStatistic(ApplicationContext.JobDbContext, msgStpCh.NewStep.JobId, msgStpCh.NewStep.StepId);
        }
    }
}
