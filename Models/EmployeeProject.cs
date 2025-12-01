using System;
using System.ComponentModel.DataAnnotations;

namespace EmployeePortal.Models
{
    public class EmployeeProject
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int EmployeeId { get; set; }
        public Employee? Employee { get; set; }

        [Required]
        public int ProjectId { get; set; }
        public Project? Project { get; set; }

        [MaxLength(100)]
        public string? Role { get; set; }  

        public DateTime AssignedOn { get; set; } = DateTime.UtcNow;

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }  

        public bool IsActive { get; set; } = true;

        public bool ReleaseRequested { get; set; } = false;

        [MaxLength(1000)]
        public string? ReleaseComment { get; set; }
    }
}
