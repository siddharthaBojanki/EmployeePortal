using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeePortal.Models
{
    public class WfhRequest
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int EmployeeId { get; set; }                 // FK -> Employees.Id

        [ForeignKey(nameof(EmployeeId))]
        public virtual Employee? Employee { get; set; }

        [Required]
        public DateTime Date { get; set; }                 

        [MaxLength(500)]
        public string? Reason { get; set; }

        public RequestStatus Status { get; set; } = RequestStatus.Pending;

        [MaxLength(500)]
        public string? AdminComment { get; set; }

        public DateTime RequestedOn { get; set; } = DateTime.UtcNow;
    }
}
