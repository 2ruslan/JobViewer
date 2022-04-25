using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JobViewer.Model
{
    [Table("sysjobsteps", Schema = "dbo")]
    public class JobStep
    {
        [Key]
        [Column("step_uid")]
        public Guid StepUId { get; set; }

        [Column("step_id")]
        public int StepId { get; set; }

        [Column("job_id")]
        public Guid JobId { get; set; }

        [Column("step_name")]
        public string Name { get; set; }

        [Column("subsystem")]
        public string Subsystem { get; set; }

        [Column("command")]
        public string Command { get; set; }

        [Column("on_success_step_id")]
        public int OnSuccessStepId { get; set; }

        [Column("on_fail_step_id")]
        public int OnFailStepId { get; set; }

        [Column("database_name")]
        public string ShemaName { get; set; }
    }
}
