using EmployeePortal.Models;
using EmployeePortal.ViewModels.EmployeeViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EmployeePortal.Controllers
{
    public partial class EmployeeController : Controller
    {
        // GET: /Employee/MyProjects
        public async Task<IActionResult> MyProjects()
        {
            var userId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Challenge();

            var employee = await _db.Employees.FirstOrDefaultAsync(e => e.UserId == userId);
            if (employee == null) return NotFound("Employee record not found.");

            var assignments = await _db.EmployeeProjects
                .Where(ap => ap.EmployeeId == employee.Id && ap.IsActive)   // only active
                .Include(ap => ap.Project)
                .OrderByDescending(ap => ap.AssignedOn)
                .ToListAsync();

            var vm = new EmployeePortal.ViewModels.EmployeeViewModels.MyProjectsViewModel
            {
                EmployeeId = employee.Id,
                FullName = employee.FullName,
                Assignments = assignments.Select(ap => new EmployeePortal.ViewModels.EmployeeViewModels.MyProjectsViewModel.ProjectAssignmentDto
                {
                    AssignmentId = ap.Id,
                    ProjectId = ap.ProjectId,
                    ProjectName = ap.Project?.Name ?? "",
                    Client = ap.Project?.Client,
                    Role = ap.Role,
                    AssignedOn = ap.AssignedOn,
                    ProjectDeadline = ap.Project?.Deadline,
                    ProjectDescription = ap.Project?.Description,
                    IsActive = ap.IsActive
                }).ToList()
            };

            return View("Projects/MyProjects", vm);
        }


        // GET: /Employee/ProjectDetails/5
        public async Task<IActionResult> ProjectDetails(int assignmentId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Challenge();

            var employee = await _db.Employees.FirstOrDefaultAsync(e => e.UserId == userId);
            if (employee == null) return NotFound();

            var ap = await _db.EmployeeProjects
                .Where(x => x.Id == assignmentId && x.EmployeeId == employee.Id)
                .Include(x => x.Project)
                .FirstOrDefaultAsync();

            if (ap == null) return NotFound();

            var dto = new MyProjectsViewModel.ProjectAssignmentDto
            {
                AssignmentId = ap.Id,
                ProjectId = ap.ProjectId,
                ProjectName = ap.Project?.Name ?? "",
                Client = ap.Project?.Client,
                Role = ap.Role,
                AssignedOn = ap.AssignedOn,
                ProjectDeadline = ap.Project?.Deadline,
                ProjectDescription = ap.Project?.Description,
                IsActive = ap.IsActive
            };

            return View("Projects/ProjectDetails", dto);
        }

        // Method to handle release request
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> RequestRelease(int assignmentId, string? comment)
        {
            var userId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Challenge();

            var employee = await _db.Employees.FirstOrDefaultAsync(e => e.UserId == userId);
            if (employee == null) return NotFound();

            var ap = await _db.EmployeeProjects
                .FirstOrDefaultAsync(x => x.Id == assignmentId && x.EmployeeId == employee.Id);

            if (ap == null) return NotFound();

            ap.ReleaseRequested = true;
            ap.ReleaseComment = string.IsNullOrWhiteSpace(comment) ? null : comment.Trim();

            _db.EmployeeProjects.Update(ap);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Release request submitted. Admin will review it.";
            return RedirectToAction(nameof(MyProjects));
        }

        // Method to mark assignment as complete
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAssignmentComplete(int assignmentId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Challenge();

            var employee = await _db.Employees.FirstOrDefaultAsync(e => e.UserId == userId);
            if (employee == null) return NotFound();

            var ap = await _db.EmployeeProjects.FindAsync(assignmentId);
            if (ap == null || ap.EmployeeId != employee.Id) return NotFound();

            ap.IsActive = false;
            ap.EndDate = DateTime.UtcNow;
            _db.EmployeeProjects.Update(ap);
            await _db.SaveChangesAsync();

            // notify admins
            var admins = await _db.Employees.Where(e => e.Role == RoleType.Admin && e.IsActive).ToListAsync();
            foreach (var admin in admins)
            {
                _db.Notifications.Add(new Notification
                {
                    EmployeeId = admin.Id,
                    Title = $"Assignment completed: {ap.Project?.Name}",
                    Message = $"{employee.FullName} marked assignment on '{ap.Project?.Name}' as complete.",
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false
                });
            }
            await _db.SaveChangesAsync();

            TempData["Success"] = "Marked as complete.";
            return RedirectToAction(nameof(MyProjects));
        }
    }
}
