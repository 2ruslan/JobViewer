using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JobViewer.Model
{
    public class TextLine
    {
        [Key]   
        [Column("text")]
        public string Text { get; set; }
    }
}
