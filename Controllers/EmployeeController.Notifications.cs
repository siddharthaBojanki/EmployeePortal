using EmployeePortal.ViewModels.AdminViewModels;
using EmployeePortal.ViewModels.EmployeeViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeePortal.Controllers
{
    public partial class EmployeeController : Controller
    {
        // GET: Employee/Notifications
        [HttpGet]
        public async Task<IActionResult> Notifications()
        {
            var user = await _userManager.GetUserAsync(User);
            var emp = await _db.Employees.FirstOrDefaultAsync(e => e.UserId == user.Id);

            var notes = await _db.Notifications
                .Where(n => n.EmployeeId == emp.Id)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            var vm = notes.Select(n => new NotificationViewModel
            {
                Id = n.Id,
                Title = n.Title,
                Message = n.Message,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt
            }).ToList();
            // Fetch announcements
            ViewBag.RecentAnnouncements = await _db.Announcements
                                                   .OrderByDescending(a => a.CreatedAt)
                                                   .Select(a => new AnnouncementSummary
                                                   {
                                                       Id = a.Id,
                                                       Title = a.Title,
                                                       Message = a.Message,
                                                       CreatedAt = a.CreatedAt
                                                   })
                                                   .ToListAsync();
            return View("Notifications/Notifications", vm);
        }

        // Method to mark notification as read
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkNotificationRead(int id)
        {
            var n = await _db.Notifications.FindAsync(id);
            if (n == null) return NotFound();
            n.IsRead = true;
            _db.Notifications.Update(n);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Notifications));
        }

        // Method to delete notification
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            var n = await _db.Notifications.FindAsync(id);
            if (n == null) return NotFound();
            _db.Notifications.Remove(n);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Notification deleted.";
            return RedirectToAction(nameof(Notifications));
        }

        // Method to delete all notifications
        [HttpPost]
        public async Task<IActionResult> ClearAllNotifications()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var emp = await _db.Employees
                .FirstOrDefaultAsync(e => e.UserId == user.Id);

            if (emp == null)
                return RedirectToAction("Dashboard");

            var notifications = _db.Notifications
                .Where(n => n.EmployeeId == emp.Id);

            _db.Notifications.RemoveRange(notifications);
            await _db.SaveChangesAsync();
            TempData["Success"] = "All Notifications deleted.";
            return RedirectToAction("Notifications");
        }
    }
}
