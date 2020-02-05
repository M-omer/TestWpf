using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace timesheet.data.Models
{
    public class Employee
    {
        [Key]
        public int EmployeeID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string TWEHours { get; set; }
        public string AWEHours { get; set; }
    }
}
