using EmployeePortal.ViewModels.EmployeeViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeePortal.Controllers
{
    public partial class EmployeeController : Controller
    {
        // GET: Employee/Performance
        [HttpGet]
        public async Task<IActionResult> Performance()
        {
            var user = await _userManager.GetUserAsync(User);
            var emp = await _db.Employees.FirstOrDefaultAsync(e => e.UserId == user.Id);

            var reviews = await _db.PerformanceReviews
                .Where(r => r.EmployeeId == emp.Id)
                .OrderByDescending(r => r.ReviewDate)
                .Include(r => r.ReviewedByEmployee)
                .ToListAsync();

            var vm = reviews.Select(r => new PerformanceViewModel
            {
                Rating = r.Rating,
                ReviewDate = r.ReviewDate,
                Feedback = r.Feedback,
                ReviewedByName = r.ReviewedByEmployee?.FullName
            }).ToList();

            return View("Performances/Performance", vm);
        }
    }
}
