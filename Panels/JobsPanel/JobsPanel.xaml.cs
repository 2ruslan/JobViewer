using JobViewer.Base;
using JobViewer.Messages;
using System.ComponentModel;

namespace JobViewer.Panels.JobsPanel
{
    public partial class JobsPanel : BaseUserControl
    {
        private JobsPanelViewModel viewModel;

        public JobsPanel()
        {
            InitializeComponent();

            DataContext = viewModel = new JobsPanelViewModel();

            viewModel.PropertyChanged += ViewModelPropertyChanged;

            ApplicationContext?.Messanger
                .Subscribe(typeof(MessageJobsLoad), OnRecieveMessage);
        }

        private void ViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(JobsPanelViewModel.CurrentJob))
                ApplicationContext.Messanger
                    .Send(new MessageJobChanged(viewModel.CurrentJob));
        }

        private void OnRecieveMessage(IMessage m)
        {
            if (m is MessageJobsLoad)
                viewModel.LoadJobs(ApplicationContext.JobDbContext);
        }
    }
}
