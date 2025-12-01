using System.ComponentModel.DataAnnotations;

namespace EmployeePortal.ViewModels.EmployeeViewModels
{
    public class EditContactModel
    {
        [MaxLength(15)]
        public string? Phone { get; set; }

        [MaxLength(250)]
        public string? Address { get; set; }
    }
}
