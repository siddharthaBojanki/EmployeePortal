using System;
using System.ComponentModel.DataAnnotations;

namespace EmployeePortal.ViewModels.AdminViewModels
{
    public class EditEmployeeModel
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        public int? DepartmentId { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateOfJoining { get; set; }

        [MaxLength(15)]
        public string? Phone { get; set; }

        [MaxLength(250)]
        public string? Address { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
