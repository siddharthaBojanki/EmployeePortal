using System;

namespace EmployeePortal.ViewModels.EmployeeViewModels
{
    public class AnnouncementViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Message { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public string? CreatedByName { get; set; }
    }
}
