using EmployeePortal.Models;

namespace EmployeePortal.ViewModels.EmployeeViewModels
{
    public class ProfileViewModel
    {
        public Employee Employee { get; set; } = null!;
        public string? Email { get; set; }
    }
}
