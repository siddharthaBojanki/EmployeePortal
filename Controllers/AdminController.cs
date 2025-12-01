using EmployeePortal.Data;
using EmployeePortal.Models;
using EmployeePortal.Services;
using EmployeePortal.ViewModels.AdminViewModels;
using EmployeePortal.ViewModels.EmployeeViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeePortal.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/[action]")]
    public partial class AdminController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly NotificationService _notificationService;

        public AdminController(ApplicationDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, NotificationService notificationService)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _notificationService = notificationService;
        }

        // GET: /Admin/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var vm = new AdminDashboardViewModel();

            vm.TotalEmployees = await _db.Employees.CountAsync();
            vm.PendingLeaves = await _db.LeaveRequests.CountAsync(l => l.Status == RequestStatus.Pending);
            vm.PendingWfhs = await _db.WfhRequests.CountAsync(w => w.Status == RequestStatus.Pending);
            vm.TotalReviews = await _db.PerformanceReviews.CountAsync();

            // Recent Leave Requests (latest 2)
            vm.RecentLeaveRequests = await _db.LeaveRequests
                .Include(l => l.Employee)
                .OrderByDescending(l => l.AppliedOn)
                .Take(2)
                .Select(l => new LeaveRequestSummary
                {
                    Id = l.Id,
                    EmployeeName = l.Employee != null ? l.Employee.FullName : "(unknown)",
                    FromDate = l.FromDate,
                    ToDate = l.ToDate,
                    LeaveType = l.LeaveType.ToString(),
                    Status = l.Status.ToString(),
                    AppliedOn = l.AppliedOn
                }).ToListAsync();

            // Recent WFH Requests (latest 2)
            vm.RecentWfhRequests = await _db.WfhRequests
                .Include(w => w.Employee)
                .OrderByDescending(w => w.RequestedOn)
                .Take(2)
                .Select(w => new WfhRequestSummary
                {
                    Id = w.Id,
                    EmployeeName = w.Employee != null ? w.Employee.FullName : "(unknown)",
                    Date = w.Date,
                    Status = w.Status.ToString(),
                    RequestedOn = w.RequestedOn
                }).ToListAsync();

            // Recent Announcements (latest 2)
            vm.RecentAnnouncements = await _db.Announcements
                .OrderByDescending(a => a.CreatedAt)
                .Take(4)
                .Select(a => new AnnouncementSummary
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
