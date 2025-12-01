using System;
using System.ComponentModel.DataAnnotations;
using EmployeePortal.Models;

namespace EmployeePortal.ViewModels.EmployeeViewModels
{
    public class ApplyLeaveModel
    {
        [Required]
        public LeaveType LeaveType { get; set; }

        [Required]
        public DateTime FromDate { get; set; }

        [Required]
        public DateTime ToDate { get; set; }

        [MaxLength(500)]
        public string? Reason { get; set; }
    }
}
