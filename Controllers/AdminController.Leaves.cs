using EmployeePortal.Data;
using EmployeePortal.Models;
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
using EmployeePortal.Services;

namespace EmployeePortal.Controllers
{
    public partial class AdminController : Controller
    {
        // GET: Admin/LeaveRequests
        public async Task<IActionResult> LeaveRequests()
        {
            var list = await _db.LeaveRequests.Include(l => l.Employee).OrderByDescending(l => l.AppliedOn).ToListAsync();
            return View("Leaves/LeaveRequests", list);
        }

        // Method to approve leave
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveLeave(int id, string? comment)
        {
            var leave = await _db.LeaveRequests.Include(l => l.Employee).FirstOrDefaultAsync(l => l.Id == id);
            if (leave == null) return NotFound();

            leave.Status = RequestStatus.Approved;
            leave.AdminComment = comment;
            await _db.SaveChangesAsync();

            await _notificationService.CreateNotification(leave.EmployeeId, "Leave approved", $"Your leave ({leave.FromDate:yyyy-MM-dd} to {leave.ToDate:yyyy-MM-dd}) was approved.");

            TempData["Success"] = "Leave approved.";
            return RedirectToAction(nameof(LeaveRequests));
        }

        // Method to reject leave
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectLeave(int id, string? comment)
        {
            var leave = await _db.LeaveRequests.Include(l => l.Employee).FirstOrDefaultAsync(l => l.Id == id);
            if (leave == null) return NotFound();

            leave.Status = RequestStatus.Rejected;
            leave.AdminComment = comment;
            await _db.SaveChangesAsync();

            await _notificationService.CreateNotification(leave.EmployeeId, "Leave rejected", $"Your leave ({leave.FromDate:yyyy-MM-dd} to {leave.ToDate:yyyy-MM-dd}) was rejected. {comment}");

            TempData["Success"] = "Leave rejected.";
            return RedirectToAction(nameof(LeaveRequests));
        }
    }
}
