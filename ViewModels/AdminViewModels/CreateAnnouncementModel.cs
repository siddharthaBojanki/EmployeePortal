using System.ComponentModel.DataAnnotations;

namespace EmployeePortal.ViewModels.AdminViewModels
{
    public class CreateAnnouncementModel
    {
        [Required, MaxLength(200)]
        public string Title { get; set; } = "";

        [Required, MaxLength(4000)]
        public string Message { get; set; } = "";
    }
}
