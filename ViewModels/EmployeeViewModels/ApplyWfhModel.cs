using System;
using System.ComponentModel.DataAnnotations;

namespace EmployeePortal.ViewModels.EmployeeViewModels
{
    public class ApplyWfhModel
    {
        [Required]
        public DateTime Date { get; set; }

        [MaxLength(500)]
        public string? Reason { get; set; }
    }
}
