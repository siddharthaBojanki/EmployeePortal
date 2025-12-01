using EmployeePortal.ViewModels.AdminViewModels;
using System;
using System.Collections.Generic;

namespace EmployeePortal.ViewModels.EmployeeViewModels
{
    public class DashboardViewModel
    {
        // Basic profile
        public int EmployeeId { get; set; }
        public string FullName { get; set; } = "";

        // --- Leaves analytics ---
        public int LeaveEntitlement { get; set; }      // total entitlement (days)
        public int LeaveUsed { get; set; }             // approved days used
        public int LeaveRemaining => Math.Max(0, LeaveEntitlement - LeaveUsed);
        public int PendingLeaveRequests { get; set; }  // number of pending leave requests

        // --- WFH analytics ---
        public int WfhEntitlement { get; set; }        // total WFH allowance (days or requests depending on policy)
        public int WfhUsed { get; set; }               // approved WFH count/days
        public int WfhRemaining => Math.Max(0, WfhEntitlement - WfhUsed);
        public int PendingWfhRequests { get; set; }

        // --- Performance ---
        public int LatestRating { get; set; }          // last rating (1-5) or 0 if none
        public DateTime? LatestRatingDate { get; set; }

        // Bottom lists
        public List<SimplePerformance> RecentPerformance { get; set; } = new();
        public List<SimpleLeave> RecentLeaves { get; set; } = new();
        public List<SimpleNotification> RecentNotifications { get; set; } = new();
        public List<AllAnnouncementsSummary> RecentAnnouncements { get; set; } = new();
    }

    public class SimplePerformance
    {
        public int Id { get; set; }
        public DateTime ReviewDate { get; set; }
        public int Rating { get; set; }
        public string? Feedback { get; set; }
    }

    public class SimpleLeave
    {
        public int Id { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public string Status { get; set; } = "";
    }

    public class SimpleNotification
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
    }
    public class AllAnnouncementsSummary
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string? Message { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
