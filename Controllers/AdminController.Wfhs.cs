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

namespace EmployeePortal.Controllers
{
    public partial class AdminController : Controller
    {
        // GET: Admin/WfhRequests
        public async Task<IActionResult> WfhRequests()
        {
            var list = await _db.WfhRequests.Include(w => w.Employee).OrderByDescending(w => w.RequestedOn).ToListAsync();
            return View("Wfhs/WfhRequests", list);
        }

        // Method to approve WFH requests
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveWfh(int id, string? comment)
        {
            var wfh = await _db.WfhRequests.Include(w => w.Employee).FirstOrDefaultAsync(w => w.Id == id);
            if (wfh == null) return NotFound();

            wfh.Status = RequestStatus.Approved;
            wfh.AdminComment = comment;
            await _db.SaveChangesAsync();

            await _notificationService.CreateNotification(wfh.EmployeeId, "WFH approved", $"Your WFH request on {wfh.Date:yyyy-MM-dd} was approved.");

            TempData["Success"] = "WFH approved.";
            return RedirectToAction(nameof(WfhRequests));
        }

        // Method to reject WFH requests
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectWfh(int id, string? comment)
        {
            var wfh = await _db.WfhRequests.Include(w => w.Employee).FirstOrDefaultAsync(w => w.Id == id);
            if (wfh == null) return NotFound();

            wfh.Status = RequestStatus.Rejected;
            wfh.AdminComment = comment;
            await _db.SaveChangesAsync();

            await _notificationService.CreateNotification(wfh.EmployeeId, "WFH rejected", $"Your WFH request on {wfh.Date:yyyy-MM-dd} was rejected. {comment}");

            TempData["Success"] = "WFH rejected.";
            return RedirectToAction(nameof(WfhRequests));
        }
    }
}
