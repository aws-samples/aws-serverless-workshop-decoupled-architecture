using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LeaveWebApp.Models
{
    public class Leave
    {
        public string LeaveID { get; set; }

        [Display(Name = "Employee ID")]
        public string EmployeeID { get; set; }

        [Display(Name = "Employee Name")]
        public string EmployeeName { get; set; }

        [Display(Name = "Leave Start Date")]
        [DataType(DataType.Date)]
        public DateTime LeaveStartDate { get; set; }

        [Display(Name = "Leave End Date")]
        [DataType(DataType.Date)]
        public DateTime LeaveEndDate { get; set; }
        public string LeaveStatus { get; set; }

    }
}
