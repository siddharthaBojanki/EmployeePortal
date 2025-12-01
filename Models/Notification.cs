using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeePortal.Models
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int EmployeeId { get; set; }                 // FK -> Employees.Id

        [ForeignKey(nameof(EmployeeId))]
        public virtual Employee? Employee { get; set; }

        [Required, MaxLength(250)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Message { get; set; }

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
