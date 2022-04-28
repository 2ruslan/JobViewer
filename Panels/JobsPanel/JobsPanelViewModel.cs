using JobViewer.Base;
using JobViewer.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobViewer.Panels.JobsPanel
{
    public class JobsPanelViewModel : NotifyPropertyChangedClass
    {
        private IList<Job> jobs;
        public IList<Job> Jobs
        {
            get => jobs;
            set
            {
                if (jobs == value)
                    return;
                jobs = value;
                NotifyPropertyChanged(nameof(Jobs));
            }
        }

        private Job currentJob;
        public Job CurrentJob
        {
            get => currentJob;
            set
            {
                if (currentJob == value)
                    return;
                currentJob = value;
                NotifyPropertyChanged(nameof(CurrentJob));
            }
        }

        public void LoadJobs(JobDbContext dbContext)
        {
            Jobs = new ObservableCollection<Job>(
                        dbContext.Jobs
                            .OrderBy(x => x.Name)
                            .ToList());
            CurrentJob = Jobs.FirstOrDefault();
        }
    }
}
