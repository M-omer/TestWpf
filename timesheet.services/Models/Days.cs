using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace timesheet.data.Models
{
   public class Days
    { 
        [Key]
        public int DayId { get; set; }

        [StringLength(255)]
        [Required]
        public string Day { get; set; }
    }
}
