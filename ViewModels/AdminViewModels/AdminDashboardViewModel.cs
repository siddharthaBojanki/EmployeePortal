using System;
using System.Collections.Generic;

namespace EmployeePortal.ViewModels.AdminViewModels
{
    public class AdminDashboardViewModel
    {
        // Top counters
        public int TotalEmployees { get; set; }
        public int PendingLeaves { get; set; }
        public int PendingWfhs { get; set; }
        public int TotalReviews { get; set; }

        // Bottom lists (recent)
        public List<LeaveRequestSummary> RecentLeaveRequests { get; set; } = new();
        public List<WfhRequestSummary> RecentWfhRequests { get; set; } = new();
        public List<PerformanceReviewSummary> RecentReviews { get; set; } = new();
        public List<AnnouncementSummary> RecentAnnouncements { get; set; } = new();
    }

    public class LeaveRequestSummary
    {
        public int Id { get; set; }
        public string EmployeeName { get; set; } = "";
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string LeaveType { get; set; } = "";
        public string Status { get; set; } = "";
        public DateTime AppliedOn { get; set; }
    }

    public class WfhRequestSummary
    {
        public int Id { get; set; }
        public string EmployeeName { get; set; } = "";
        public DateTime Date { get; set; }
        public string Status { get; set; } = "";
        public DateTime RequestedOn { get; set; }
    }

    public class PerformanceReviewSummary
    {
        public int Id { get; set; }
        public string EmployeeName { get; set; } = "";
        public DateTime ReviewDate { get; set; }
        public int Rating { get; set; }
        public string? Feedback { get; set; }
    }

    public class AnnouncementSummary
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string? Message { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
