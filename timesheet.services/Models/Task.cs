using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace timesheet.data.Models
{
    public class Task
    {
        [Key]
        public int TaskID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<TimeSheet> TimeSheets { get; set; }
        public ICollection<TaskEmployee> TaskEmployees { get; set; }
    }
}
