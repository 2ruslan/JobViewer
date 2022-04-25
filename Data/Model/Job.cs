using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JobViewer.Model
{
    [Table("sysjobs", Schema = "dbo")]
    public class Job
    {
        [Key]
        [Column("job_id")]
        public Guid JobId { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("description")]
        public string Description { get; set; }
        
        [Column("enabled")]
        public byte Enabled { get; set; }

        [NotMapped]
        public bool IsEnabled
        {
            get => Enabled == 1;
        }
        
    }
}
