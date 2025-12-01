using EmployeePortal.Models;
using EmployeePortal.ViewModels.EmployeeViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmployeePortal.Services;

namespace EmployeePortal.Controllers
{
    public partial class EmployeeController : Controller
    {
        // GET: Employee/ApplyWfh
        [HttpGet]
        public IActionResult ApplyWfh()
        {
            var model = new ApplyWfhModel { Date = DateTime.UtcNow.Date };
            return View("Wfhs/ApplyWfh", model);
        }

        // POST: Employee/ApplyWfh
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApplyWfh(ApplyWfhModel model)
        {
            if (!ModelState.IsValid)
                return View("Wfhs/ApplyWfh", model);

            var user = await _userManager.GetUserAsync(User);
            var emp = await _db.Employees.FirstOrDefaultAsync(e => e.UserId == user.Id);

            var wfh = new WfhRequest
            {
                EmployeeId = emp.Id,
                Date = model.Date,
                Reason = model.Reason,
                Status = RequestStatus.Pending,
                RequestedOn = DateTime.UtcNow
            };

            _db.WfhRequests.Add(wfh);
            await _db.SaveChangesAsync();

            await _notificationService.CreateNotification(emp.Id, "WFH requested", $"WFH on {model.Date:yyyy-MM-dd} submitted.");

            TempData["Success"] = "WFH request submitted.";
            return RedirectToAction(nameof(WfhHistory));
        }

        // GET: Employee/WfhHistory 
        [HttpGet]
        public async Task<IActionResult> WfhHistory()
        {
            var user = await _userManager.GetUserAsync(User);
            var emp = await _db.Employees.FirstOrDefaultAsync(e => e.UserId == user.Id);

            var list = await _db.WfhRequests.Where(w => w.EmployeeId == emp.Id)
                                            .OrderByDescending(w => w.RequestedOn)
                                            .ToListAsync();
            return View("Wfhs/WfhHistory", list);
        }

        // Method to handle wfh cancel request
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelWfh(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var emp = await _db.Employees.FirstOrDefaultAsync(e => e.UserId == user.Id);
            var wfh = await _db.WfhRequests.FirstOrDefaultAsync(w => w.Id == id && w.EmployeeId == emp.Id);
            if (wfh == null) return NotFound();
            if (wfh.Status != RequestStatus.Pending)
            {
                TempData["Error"] = "Only pending WFH requests can be cancelled.";
                return RedirectToAction(nameof(WfhHistory));
            }

            _db.WfhRequests.Remove(wfh);
            await _db.SaveChangesAsync();

            await _notificationService.CreateNotification(emp.Id, "WFH cancelled", $"Your WFH request (id:{id}) was cancelled.");

            TempData["Success"] = "WFH request cancelled.";
            return RedirectToAction(nameof(WfhHistory));
        }
    }
}
