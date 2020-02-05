using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace timesheet.data.Models
{
    public class TimeSheet
    {
        [Key]
        public int TimesheetID { get; set; }
        public string Date { get; set; }
        public string Hours { get; set; }
        public virtual Days Day { get; set; }
        public string Name { get; set; }
        public virtual Task Task { get; set; }
        public virtual Employee Employee { get; set; }
    }
}
