using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeePortal.Models
{
    public class PerformanceReview
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int EmployeeId { get; set; }                 // employee being reviewed (FK -> Employees.Id)

        [ForeignKey(nameof(EmployeeId))]
        public virtual Employee? Employee { get; set; }

        public DateTime ReviewDate { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }                   

        [MaxLength(1000)]
        public string? Feedback { get; set; }

        public int? ReviewedBy { get; set; }

        [ForeignKey(nameof(ReviewedBy))]
        public virtual Employee? ReviewedByEmployee { get; set; }
    }
}
