using JobViewer.Base;
using JobViewer.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JobViewer.Panels.StepsPanel
{
    public class StepsPanelViewModel : NotifyPropertyChangedClass
    {
        private IList<JobStep> steps;
        public IList<JobStep> Steps 
        {
            get => steps;
            set
            {
                if (steps == value)
                    return;
                steps = value;
                NotifyPropertyChanged(nameof(Steps));
            }
        }

        private JobStep currentStep;
        public JobStep CurrentStep
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

        public  void LoadJobSteps(JobDbContext dbContext, Guid jobId)
        {
            Steps = dbContext.JobSteps
                                    .Where(x => x.JobId == jobId)
                                    .OrderBy(x => x.StepId)
                                    .ToList();

            CurrentStep = Steps.FirstOrDefault();
        }
    }
}
