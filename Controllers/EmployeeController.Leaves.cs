using EmployeePortal.Models;
using EmployeePortal.ViewModels.EmployeeViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmployeePortal.Services;

namespace EmployeePortal.Controllers
{
    public partial class EmployeeController : Controller
    {
        // GET: Employee/ApplyLeave
        [HttpGet]
        public IActionResult ApplyLeave()
        {
            var model = new ApplyLeaveModel
            {
                FromDate = DateTime.UtcNow.Date,
                ToDate = DateTime.UtcNow.Date,
                LeaveType = LeaveType.Casual
            };
            return View("Leaves/ApplyLeave", model);
        }

        // POST: Employee/ApplyLeave
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApplyLeave(ApplyLeaveModel model)
        {
            if (!ModelState.IsValid)
                return View("Leaves/ApplyLeave", model);

            if (model.ToDate < model.FromDate)
            {
                ModelState.AddModelError("", "To date cannot be earlier than From date.");
                return View("Leaves/ApplyLeave", model);
            }

            var user = await _userManager.GetUserAsync(User);
            var emp = await _db.Employees.FirstOrDefaultAsync(e => e.UserId == user.Id);
            if (emp == null) return NotFound();

            // calculating requested days
            var requestedDays = (int)((model.ToDate.Date - model.FromDate.Date).TotalDays) + 1;
            if (requestedDays <= 0) requestedDays = 1;

            // getting leave balance using the service
            var balance = await _leaveBalanceService.CalculateLeaveBalance(emp.Id);

            // calculating remaining days for requested LeaveType
            int remaining = model.LeaveType switch
            {
                LeaveType.Casual => balance.CasualLeaves - balance.CasualUsed,
                LeaveType.Sick => balance.SickLeaves - balance.SickUsed,
                _ => balance.OtherLeaves - balance.OtherUsed
            };

            if (remaining < requestedDays)
            {
                ModelState.AddModelError("", $"Only {remaining} {model.LeaveType} Leave days remaining. Request exceeds available balance.");
                
                return View("Leaves/ApplyLeave", model);
            }

            // create leave request (still Pending)
            var leave = new LeaveRequest
            {
                EmployeeId = emp.Id,
                LeaveType = model.LeaveType,
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                Reason = model.Reason,
                Status = RequestStatus.Pending,
                AppliedOn = DateTime.UtcNow
            };

            _db.LeaveRequests.Add(leave);
            await _db.SaveChangesAsync();

            // create notification
            await _notificationService.CreateNotification(emp.Id,
                $"Leave requested ({model.LeaveType})",
                $"Leave from {model.FromDate:yyyy-MM-dd} to {model.ToDate:yyyy-MM-dd} submitted.");

            TempData["Success"] = "Leave applied successfully.";
            return RedirectToAction(nameof(LeaveHistory));
        }

        // GET: Employee/LeaveHistory
        [HttpGet]
        public async Task<IActionResult> LeaveHistory()
        {
            var user = await _userManager.GetUserAsync(User);
            var emp = await _db.Employees.FirstOrDefaultAsync(e => e.UserId == user.Id);
            if (emp == null) return NotFound();

            var leaves = await _db.LeaveRequests.Where(l => l.EmployeeId == emp.Id)
                                               .OrderByDescending(l => l.AppliedOn)
                                               .ToListAsync();

            var balance = await _leaveBalanceService.CalculateLeaveBalance(emp.Id);

            var vm = new LeaveHistoryViewModel
            {
                Leaves = leaves,
                LeaveBalance = balance
            };
            return View("Leaves/LeaveHistory", vm);
        }

        // Method to cancel leave
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelLeave(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var emp = await _db.Employees.FirstOrDefaultAsync(e => e.UserId == user.Id);
            var leave = await _db.LeaveRequests.FirstOrDefaultAsync(l => l.Id == id && l.EmployeeId == emp.Id);
            if (leave == null) return NotFound();
            if (leave.Status != RequestStatus.Pending)
            {
                TempData["Error"] = "Only pending leaves can be cancelled.";
                return RedirectToAction(nameof(LeaveHistory));
            }

            _db.LeaveRequests.Remove(leave);
            await _db.SaveChangesAsync();

            await _notificationService.CreateNotification(emp.Id, "Leave cancelled", $"Your leave request (id:{id}) was cancelled.");

            TempData["Success"] = "Leave cancelled.";
            return RedirectToAction(nameof(LeaveHistory));
        }
    }
}
