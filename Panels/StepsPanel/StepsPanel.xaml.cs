using JobViewer.Base;
using JobViewer.Messages;
using System.ComponentModel;

namespace JobViewer.Panels.StepsPanel
{
    public partial class StepsPanel : BaseUserControl
    {
        private StepsPanelViewModel viewModel;

        public StepsPanel()
        {
            InitializeComponent();

            DataContext = viewModel = new StepsPanelViewModel();

            viewModel.PropertyChanged += ViewModelPropertyChanged;
            
            ApplicationContext?.Messanger
                .Subscribe(typeof(MessageJobChanged), OnRecieveMessage);
        }

        private void ViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(StepsPanelViewModel.CurrentStep))
                ApplicationContext.Messanger
                    .Send(new MessageStepChanged(viewModel.CurrentStep));
        }

        private void OnRecieveMessage(IMessage m)
        {
            if (m is MessageJobChanged message)
                viewModel.LoadJobSteps(ApplicationContext.JobDbContext, message.NewJob.JobId);
        }
    }
}
