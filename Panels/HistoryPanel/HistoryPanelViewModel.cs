using JobViewer.Base;
using JobViewer.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobViewer.Panels.HistoryPanel
{
    public class HistoryPanelViewModel : NotifyPropertyChangedClass
    {
        private JobDbContext jobDbContext;
        private JobStep lastStep;

        private IList<JobHistory> history;
        public IList<JobHistory> History
        {
            get => history;
            set
            {
                if (history == value)
                    return;
                history = value;
                NotifyPropertyChanged(nameof(History));
            }
        }

        private JobHistory currentStep;
        public JobHistory CurrentStep
        {
            get => currentStep;
            set
            {
                if (currentStep == value)
                    return;
                currentStep = value;
                NotifyPropertyChanged(nameof(CurrentStep));
            }
        }


        private bool isShowHistoryByStep;
        public bool IsShowHistoryByStep
        {
            get => isShowHistoryByStep;
            set
            {
                if (value == isShowHistoryByStep)
                    return;
                isShowHistoryByStep = value;
                NotifyPropertyChanged(nameof(IsShowHistoryByStep));
                LoadLastJobHistory();
            }
        }

        public void LoadJobHistory(JobDbContext jobDbContext, JobStep step)
        {
            if (jobDbContext == null || step == null)
            {
                History.Clear();
                return;
            }

            this.jobDbContext = jobDbContext;

            if (IsShowHistoryByStep)
                LoadJobHistoryInner(jobDbContext, step.JobId, step.StepId);
            else
                LoadJobHistoryInner(jobDbContext, step.JobId);

            lastStep = step;
        }

        public void LoadLastJobHistory()
        {
            if (jobDbContext != null)
                LoadJobHistory(jobDbContext, lastStep);
        }

        private void LoadJobHistoryInner(JobDbContext jobDbContext, Guid jobId)
        {
            History?.Clear();

            History = new ObservableCollection<JobHistory>(
                    jobDbContext.JobHistory
                                            .Where(x => x.JobId == jobId)
                                            .ToList()
                                            );

            CurrentStep = History.FirstOrDefault();
        }

        private void LoadJobHistoryInner(JobDbContext jobDbContext, Guid jobId, int stepID)
        {
            History?.Clear();


            History = new ObservableCollection<JobHistory>(
                    GetHistory(jobDbContext.JobHistory
                                            .Where(x => x.JobId == jobId && x.StepId == stepID)
                                            .ToList()
                                            ));

            CurrentStep = History.FirstOrDefault();
        }

        private List<JobHistory> GetHistory(List<JobHistory> history)
        {
            SetFakeRunId(history);
            return history.OrderByDescending(x => x.FakeRunId).ThenBy(x => x.Instanceid).ToList();
        }

        private void SetFakeRunId(List<JobHistory> history)
        {
            var delta = new TimeSpan(0, 0, 1);

            var currentRunId = 0;
            for (int i = 1; i < history.Count; i++)
            {
                if (history[i].RunDateTime - history[i - 1].RunDateTimeEnd > delta)
                    currentRunId++;

                history[i].FakeRunId = currentRunId;
            }
        }

    }
}
