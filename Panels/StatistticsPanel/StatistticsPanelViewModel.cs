using JobViewer.Base;
using JobViewer.Common;
using JobViewer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobViewer.Panels.StatistticsPanel
{
    public class StatistticsPanelViewModel : NotifyPropertyChangedClass
    {
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

        public void CreateStepStatistic(JobDbContext jobDbContext, Guid? jobId, int? stepId)
        {
            double[] dataY = new double[0];
            double[] dataX = new double[0];

            if (jobId != null && stepId != null)
            {
                var stepData = jobDbContext.JobHistory
                    .Where(x => x.JobId == jobId.Value && x.StepId == stepId.Value)
                    .OrderBy(x => x.Instanceid)
                    .ToList()
                    ;

                dataY = stepData.Select(x => x.RunDurationTime.TotalSeconds / 60.0).ToArray();
                dataX = stepData.Select(x => (double)x.RunDateTime.ToOADate()).ToArray();
            }

            StepStatistic = new StatisticInfo() { DataX = dataX, DataY = dataY };
        }

        public void CreateJobStatistic(JobDbContext jobDbContext, Guid? jobId)
        {
            double[] dataY = new double[0];
            double[] dataX = new double[0];

            double[] dataAvgY = new double[0];
            double[] dataAvgX = new double[0];

            if (jobId != null)
            {
                var history = jobDbContext.JobHistory
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

        class StatisticsRow
        {
            public int Id { get; set; }
            public DateTime RunDateTime { get; set; }
            public double RunDurationSeconds { get; set; }
        }

    }
}
