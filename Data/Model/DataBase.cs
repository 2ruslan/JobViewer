using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JobViewer.Model
{
    [Table("databases", Schema = "sys")]
    public class DataBase
    {
        [Key]
        [Column("database_id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }
    }
}
