using JobViewer.Messages;
using JobViewer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobViewer.Common
{
    public class ApplicationContext
    {
        public Messanger Messanger { get; }
        public JobDbContext JobDbContext { get; }

        public ApplicationContext(Messanger messanger, JobDbContext dbContext)
        {
            Messanger = messanger;
            JobDbContext = dbContext;
        }
    }
}
