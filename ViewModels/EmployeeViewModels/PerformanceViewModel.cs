using System;

namespace EmployeePortal.ViewModels.EmployeeViewModels
{
    public class PerformanceViewModel
    {
        public DateTime ReviewDate { get; set; }
        public int Rating { get; set; }
        public string? Feedback { get; set; }
        public string? ReviewedByName { get; set; }
    }
}
