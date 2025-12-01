using System;
using System.ComponentModel.DataAnnotations;

namespace EmployeePortal.ViewModels.AdminViewModels
{
    public class CreateEmployeeModel
    {
        [Required, MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(6)]
        public string Password { get; set; } = string.Empty;

        public int? DepartmentId { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateOfJoining { get; set; } = DateTime.UtcNow.Date;

        [MaxLength(15)]
        public string? Phone { get; set; }

        [MaxLength(250)]
        public string? Address { get; set; }
    }
}
