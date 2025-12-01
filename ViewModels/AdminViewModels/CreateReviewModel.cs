using System;
using System.ComponentModel.DataAnnotations;

namespace EmployeePortal.ViewModels.AdminViewModels
{
    public class CreateReviewModel
    {
        [Required]
        public int? EmployeeId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime ReviewDate { get; set; } = DateTime.UtcNow.Date;

        [Range(1, 5)]
        public int Rating { get; set; } = 3;

        [MaxLength(1000)]
        public string? Feedback { get; set; }

        // optional admin reviewer (Employee.Id)
        public int? ReviewedBy { get; set; }
    }
}
