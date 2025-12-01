using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeePortal.Models
{
    public class LeaveRequest
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int EmployeeId { get; set; }                 // FK -> Employees.Id

        [ForeignKey(nameof(EmployeeId))]
        public virtual Employee? Employee { get; set; }

        [Required]
        public LeaveType LeaveType { get; set; }

        [Required]
        public DateTime FromDate { get; set; }

        [Required]
        public DateTime ToDate { get; set; }

        [MaxLength(500)]
        public string? Reason { get; set; }

        public RequestStatus Status { get; set; } = RequestStatus.Pending;

        [MaxLength(500)]
        public string? AdminComment { get; set; }

        public DateTime AppliedOn { get; set; } = DateTime.UtcNow;
    }
}
