using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JobViewer.Model
{
    [NotMapped]
    public class TextLine
    {
        public string Text { get; set; }
    }
}
