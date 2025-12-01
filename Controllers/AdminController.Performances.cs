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
        // GET: Admin/Reviews
        public async Task<IActionResult> Reviews()
        {
            var list = await _db.PerformanceReviews
                .Include(r => r.Employee)
                .Include(r => r.ReviewedByEmployee)
                .OrderByDescending(r => r.ReviewDate)
                .ToListAsync();

            return View("Performances/Reviews", list);
        }

        // GET: Admin/CreateReview
        public IActionResult CreateReview(int? employeeId)
        {
            ViewBag.Employees = _db.Employees.OrderBy(e => e.FullName).ToList();
            var vm = new CreateReviewModel { EmployeeId = employeeId };
            return View("Performances/CreateReview", vm);
        }

        // POST: Admin/CreateReview
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateReview(CreateReviewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Employees = _db.Employees.OrderBy(e => e.FullName).ToList();
                return View("Performances/CreateReview", model);
            }

            var review = new PerformanceReview
            {
                EmployeeId = model.EmployeeId!.Value,
                ReviewDate = model.ReviewDate,
                Rating = model.Rating,
                Feedback = model.Feedback,
                ReviewedBy = model.ReviewedBy
            };

            _db.PerformanceReviews.Add(review);
            await _db.SaveChangesAsync();

            await _notificationService.CreateNotification(model.EmployeeId.Value, "Performance review added", $"A performance review has been added (Rating: {model.Rating}).");

            TempData["Success"] = "Review added.";
            return RedirectToAction(nameof(Reviews));
        }

        // Method to delete review
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var r = await _db.PerformanceReviews.FindAsync(id);
            if (r == null) return NotFound();
            _db.PerformanceReviews.Remove(r);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Review deleted.";
            return RedirectToAction(nameof(Reviews));
        }
    }
}
