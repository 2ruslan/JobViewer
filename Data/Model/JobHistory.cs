using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JobViewer.Model
{
    [Table("sysjobhistory", Schema = "dbo")]
    public class JobHistory
    {
        [Key]
        [Column("instance_id")]
        public int Instanceid { get; set; }

        [Column("step_id")]
        public int StepId { get; set; }

        [Column("job_id")]
        public Guid JobId { get; set; }

        [Column("step_name")]
        public string StepName { get; set; }

        [Column("message")]
        public string Message { get; set; }

        [Column("run_status")]
        public int RunStatus { get; set; }
        
        [Column("run_date")]
        public int RunDate { get; set; }

        [Column("run_time")]
        public int RunTime { get; set; }

        [Column("run_duration")]
        public int RunDuration { get; set; }

        [NotMapped]
        public int FakeRunId { get; set; }

        [NotMapped]
        private DateTime? runDateTime;
        [NotMapped]
        public DateTime RunDateTime
            => runDateTime.HasValue ? runDateTime.Value : (runDateTime =GetRunDateTime()).Value ;

        [NotMapped]
        private DateTime? runDateTimeEnd;
        [NotMapped]
        public DateTime RunDateTimeEnd
            => runDateTimeEnd.HasValue ? runDateTimeEnd.Value : (runDateTimeEnd = RunDateTime + RunDurationTime).Value;

        [NotMapped]
        private TimeSpan? runDurationTime;
        [NotMapped]
        public TimeSpan RunDurationTime
            => runDurationTime.HasValue ? runDurationTime.Value : (runDurationTime = GetRunDurationTime()).Value;

        private DateTime GetRunDateTime()
        { 
            var d = RunDate % 100;
            var parts = RunDate / 100;
            var m = parts % 100;
            var y = parts / 100;

            var s = RunTime % 100;
            parts = RunTime / 100;
            var mi = parts % 100;
            var h = parts / 100;

            return new DateTime(y, m, d, h, mi, s);
        }

        private TimeSpan GetRunDurationTime()
        {

            var s = RunDuration % 100;
            var parts = RunDuration / 100;
            var mi = parts % 100;
            var h = parts / 100;

            return new TimeSpan(h, mi, s);
        }
    }
}
