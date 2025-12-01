using EmployeePortal.Data;
using EmployeePortal.Models;
using EmployeePortal.Services;
using EmployeePortal.ViewModels;
using EmployeePortal.ViewModels.AdminViewModels;
using EmployeePortal.ViewModels.EmployeeViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApp.Services;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EmployeePortal.Controllers
{
    [Authorize(Roles = "Employee,Admin")]
    public partial class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly NotificationService _notificationService;
        private readonly LeaveBalanceService _leaveBalanceService;

        public EmployeeController(ApplicationDbContext db, UserManager<ApplicationUser> userManager, NotificationService notificationService, LeaveBalanceService leaveBalanceService)
        {
            _db = db;
            _userManager = userManager;
            _notificationService = notificationService;
            _leaveBalanceService = leaveBalanceService;
        }

        // GET: Employee/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var emp = await _db.Employees
                .FirstOrDefaultAsync(e => e.UserId == user.Id);

            if (emp == null) return NotFound("Employee profile not found.");

            var vm = new DashboardViewModel
            {
                EmployeeId = emp.Id,
                FullName = emp.FullName
            };

            const int leaveEntitlement = 25; // days per year
            const int wfhEntitlement = 12;   // allowed WFH days

            vm.LeaveEntitlement = leaveEntitlement;
            vm.WfhEntitlement = wfhEntitlement;

            // ---- Leave used (approved days) and pending count ----
            var approvedLeaves = await _db.LeaveRequests
                .Where(l => l.EmployeeId == emp.Id && l.Status == RequestStatus.Approved)
                .ToListAsync();

            vm.LeaveUsed = approvedLeaves.Sum(l => (int)((l.ToDate.Date - l.FromDate.Date).TotalDays + 1));

            vm.PendingLeaveRequests = await _db.LeaveRequests
                .CountAsync(l => l.EmployeeId == emp.Id && l.Status == RequestStatus.Pending);

            // ---- WFH used and pending (count of approved/pending requests) ----
            vm.WfhUsed = await _db.WfhRequests
                .Where(w => w.EmployeeId == emp.Id && w.Status == RequestStatus.Approved)
                .CountAsync();

            vm.PendingWfhRequests = await _db.WfhRequests
                .CountAsync(w => w.EmployeeId == emp.Id && w.Status == RequestStatus.Pending);

            // ---- Latest performance rating ----
            var latestReview = await _db.PerformanceReviews
                .Where(r => r.EmployeeId == emp.Id)
                .OrderByDescending(r => r.ReviewDate)
                .FirstOrDefaultAsync();

            if (latestReview != null)
            {
                vm.LatestRating = latestReview.Rating;
                vm.LatestRatingDate = latestReview.ReviewDate;
            }
            else
            {
                vm.LatestRating = 0;
                vm.LatestRatingDate = null;
            }

            // ---- Recent performance list (1) ----
            vm.RecentPerformance = await _db.PerformanceReviews
                .Where(r => r.EmployeeId == emp.Id)
                .OrderByDescending(r => r.ReviewDate)
                .Take(1)
                .Select(r => new SimplePerformance
                {
                    Id = r.Id,
                    ReviewDate = r.ReviewDate,
                    Rating = r.Rating,
                    Feedback = r.Feedback
                }).ToListAsync();

            // ---- Recent leaves (2) ----
            vm.RecentLeaves = await _db.LeaveRequests
                .Where(l => l.EmployeeId == emp.Id)
                .OrderByDescending(l => l.AppliedOn)
                .Take(2)
                .Select(l => new SimpleLeave
                {
                    Id = l.Id,
                    From = l.FromDate,
                    To = l.ToDate,
                    Status = l.Status.ToString()
                }).ToListAsync();

            // ---- Recent notifications (3) ----
            vm.RecentNotifications = await _db.Notifications
                .Where(n => n.EmployeeId == emp.Id)
                .OrderByDescending(n => n.CreatedAt)
                .Take(3)
                .Select(n => new SimpleNotification
                {
                    Id = n.Id,
                    Title = n.Title,
                    CreatedAt = n.CreatedAt,
                    IsRead = n.IsRead
                }).ToListAsync();

            // ---- Recent announcements (4) ----
            vm.RecentAnnouncements = await _db.Announcements
                .OrderByDescending(a => a.CreatedAt)
                .Take(4)
                .Select(a => new AllAnnouncementsSummary
                {
                    Id = a.Id,
                    Title = a.Title,
                    Message = a.Message,
                    CreatedAt = a.CreatedAt
                })
                .ToListAsync();

            return View(vm);
        }
    }
}
