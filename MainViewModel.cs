using JobViewer.Base;
using JobViewer.Common;
using JobViewer.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace JobViewer
{
    class MainViewModel : NotifyPropertyChangedClass
    {
        #region Props
        public JobDbContext JobDbContext { get; }

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
                LoadJobHistory();
            }
        }

        private StatisticInfo stepStatistic;
        public StatisticInfo StepStatistic 
        {
            get => stepStatistic;
            set
            {
                stepStatistic = value;
                NotifyPropertyChanged(nameof(StepStatistic));
            }
        }

        private StatisticInfo jobStatistic;
        public StatisticInfo JobStatistic
        {
            get => jobStatistic;
            set
            {
                jobStatistic = value;
                NotifyPropertyChanged(nameof(JobStatistic));
            }
        }

        private StatisticInfo jobStepAvgStatistic;
        public StatisticInfo JobStepAvgStatistic
        {
            get => jobStepAvgStatistic;
            set
            {
                jobStepAvgStatistic = value;
                NotifyPropertyChanged(nameof(JobStepAvgStatistic));
            }
        }

        public NotifiedClass<Job> Jobs { get; private set; }
        
        public NotifiedClass<JobStep> JobSteps { get; private set; }

        public NotifiedClass<JobHistory> JobHistory { get; private set; }

        #endregion Props

        public MainViewModel()
        {
            JobDbContext = new JobDbContext();

            Jobs = new NotifiedClass<Job>(OnCurrentJobChanged);
            JobSteps = new NotifiedClass<JobStep>(OnCurrentJobStepsChanged);
            JobHistory = new NotifiedClass<JobHistory>(OnCurrentJobHistoryChanged);

            LoadJobs();
        }

        #region OnChanged

        private void OnCurrentJobChanged()
        {
            
            LoadJobHistory();
            LoadJobSteps(Jobs.Current.JobId);
            CreateJobStatistic(Jobs.Current.JobId);
        }

        private void OnCurrentJobStepsChanged()
        {
            if (IsShowHistoryByStep)
                LoadJobHistory();

            CreateStepStatistic(JobSteps.Current?.JobId, JobSteps.Current?.StepId);
        }

        private void OnCurrentJobHistoryChanged()
        {

        }

        #endregion OnChanged

        #region LoadData

        private void LoadJobs()
        {
            Jobs.Collection = new ObservableCollection<Job>(
                JobDbContext.Jobs
                            .OrderBy(x => x.Name)
                            .ToList());
            Jobs.Current = Jobs.Collection.FirstOrDefault();
        }

        private void LoadJobSteps(Guid jobId)
        {
            var steps = JobDbContext.JobSteps
                                    .Where(x => x.JobId == jobId)
                                    .OrderBy(x => x.StepId)
                                    .ToArray();

            JobSteps.Collection = new ObservableCollection<JobStep>(steps);
            JobSteps.Current = JobSteps.Collection.FirstOrDefault();
        }

        public void LoadJobHistory()
        {
            if (IsShowHistoryByStep)
                LoadJobHistory(JobSteps.Current?.JobId, JobSteps.Current?.StepId);
            else
                LoadJobHistory(Jobs.Current?.JobId);
        }
        private void LoadJobHistory(Guid? jobId)
        {
            if (jobId == null)
                JobHistory.Collection.Clear();
            else
                JobHistory.Collection = new ObservableCollection<JobHistory>(
                    GetHistory(JobDbContext.JobHistory
                                            .Where(x => x.JobId == jobId)
                                            .ToList()
                                            ));
            JobHistory.Current = JobHistory.Collection.FirstOrDefault();
        }

        private  void LoadJobHistory(Guid? jobId, int? stepId)
        {
            if (jobId == null || stepId == null)
                JobHistory.Collection.Clear();
            else
                JobHistory.Collection = new ObservableCollection<JobHistory>(
                    GetHistory(JobDbContext.JobHistory
                                            .Where(x => x.JobId == jobId && x.StepId == stepId)
                                            .ToList()
                                            ));
            
            JobHistory.Current = JobHistory.Collection.FirstOrDefault();
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

        #endregion LoadData

        #region Statistic

        private void CreateStepStatistic(Guid? jobId, int? stepId)
        {
            double[] dataY = new double[0];
            double[] dataX = new double[0];

            if (JobHistory.Collection != null && jobId != null && stepId != null)
            {
                var stepData = JobHistory.Collection
                    .Where(x => x.JobId == jobId.Value && x.StepId == stepId.Value)
                    .OrderBy(x => x.Instanceid)
                    .ToList()
                    ;

                dataY = stepData.Select(x => x.RunDurationTime.TotalSeconds / 60.0).ToArray();
                dataX = stepData.Select(x => (double)x.RunDateTime.ToOADate()).ToArray();
            }
            
            StepStatistic = new StatisticInfo() { DataX = dataX, DataY = dataY };
        }

        private void CreateJobStatistic(Guid? jobId)
        {
            double[] dataY = new double[0];
            double[] dataX = new double[0];

            double[] dataAvgY = new double[0];
            double[] dataAvgX = new double[0];

            if (JobHistory.Collection != null && jobId != null)
            {
                var history = JobDbContext.JobHistory
                                            .Where(x => x.JobId == jobId.Value)
                                            .OrderBy(x => x.Instanceid)
                                            .ToList()
                                            ;

                SetFakeRunId(history);
                
                // total 
                var historyX = history
                                .GroupBy(x => x.FakeRunId)
                                .SelectMany(z => z.Select(
                                    csLine => new StatisticsRow
                                    {
                                        Id = csLine.FakeRunId,
                                        RunDateTime = z.Min(c => c.RunDateTime),
                                        RunDurationSeconds = z.Sum(c => c.RunDurationTime.TotalSeconds)
                                    })
                                ).ToList();

                dataY = historyX.Select(x => x.RunDurationSeconds / 60.0).ToArray();
                dataX = historyX.Select(x => (double)x.RunDateTime.ToOADate()).ToArray();

                // avg 
                var historyAvg = history
                               .Where(x => x.StepId > 0)
                               .GroupBy(x => x.StepId)
                               .SelectMany(z => z.Select(
                                   csLine => new StatisticsRow
                                   {
                                       Id = csLine.StepId,
                                       RunDurationSeconds = z.Average(c => c.RunDurationTime.TotalSeconds)
                                   })
                               ).ToList();

                dataAvgY = historyAvg.Select(x => x.RunDurationSeconds / 60.0).ToArray();
                dataAvgX = historyAvg.Select(x => (double)x.Id).ToArray();
            }

            JobStatistic = new StatisticInfo() { DataX = dataX, DataY = dataY };
            JobStepAvgStatistic = new StatisticInfo() { DataX = dataAvgX, DataY = dataAvgY };

        }

        class StatisticsRow
        {
            public int Id{ get; set; }
            public DateTime RunDateTime { get; set; }
            public double RunDurationSeconds { get; set; }
        }

        #endregion Statistic
    }
}
