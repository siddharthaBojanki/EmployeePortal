using System;

namespace EmployeePortal.ViewModels.EmployeeViewModels
{
    public class NotificationViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Message { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
