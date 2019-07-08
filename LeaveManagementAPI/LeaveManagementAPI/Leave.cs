using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LeaveManagementAPI
{
    public class Leave
    {
        public string LeaveID { get; set; }

        public string EmployeeID { get; set; }

        [Display(Name = "Employee Name")]
        public string EmployeeName { get; set; }

        [Display(Name = "Manager Name")]
        public string ManagerName { get; set; }

        [Display(Name = "Leave Start Date")]
        public DateTime LeaveStartDate { get; set; }

        [Display(Name = "Leave End Date")]
        public DateTime LeaveEndDate { get; set; }
        public string Department { get; set; }
        public string LeaveStatus { get; set; }

    }
}
