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
        // GET: Admin/Announcements
        public async Task<IActionResult> Announcements()
        {
            var announcements = await _db.Announcements
                                        .OrderByDescending(a => a.CreatedAt)
                                        .ToListAsync();

            var vm = announcements.Select(a => new AnnouncementViewModel
            {
                Id = a.Id,
                Title = a.Title,
                Message = a.Message,
                CreatedAt = a.CreatedAt,
            }).ToList();

            return View("Announcements/Index", vm);
        }

        // GET: Admin/CreateAnnouncement
        [HttpGet]
        public IActionResult CreateAnnouncement()
        {
            return View("Announcements/Create", new CreateAnnouncementModel());
        }

        // POST: Admin/CreateAnnouncement
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAnnouncement(CreateAnnouncementModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Announcements/Create", model);
            }

            // create announcement
            var ann = new Announcement
            {
                Title = model.Title,
                Message = model.Message,
                CreatedAt = DateTime.UtcNow,
            };

            _db.Announcements.Add(ann);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Announcement sent to all employees.";

            return RedirectToAction(nameof(Announcements));
        }

        // Method to delete announcement
        [HttpPost]
        public async Task<IActionResult> DeleteAnnouncement(int id)
        {
            var a = await _db.Announcements.FindAsync(id);
            if (a != null)
            {
                _db.Announcements.Remove(a);
                await _db.SaveChangesAsync();
            }
            TempData["Success"] = "Announcement deleted.";
            return RedirectToAction(nameof(Announcements));
        }
    }
}
