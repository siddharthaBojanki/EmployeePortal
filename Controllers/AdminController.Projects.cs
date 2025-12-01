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
        // GET: /Admin/Projects
        public async Task<IActionResult> Projects()
        {
            var projects = await _db.Projects
                                    .Include(p => p.Assignments)
                                    .ToListAsync();
            return View("Projects/Projects", projects);
        }

        // GET: /Admin/CreateProject
        public IActionResult CreateProject()
        {
            return View("Projects/CreateProject", new Project { StartDate = DateTime.UtcNow.Date });
        }

        // POST: /Admin/CreateProject
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProject(Project model)
        {
            if (!ModelState.IsValid) return View("Projects/CreateProject", model);

            if (model.Deadline.HasValue && model.StartDate.HasValue && model.Deadline < model.StartDate)
            {
                ModelState.AddModelError("Deadline", "Deadline cannot be earlier than start date.");
                return View("Projects/CreateProject", model);
            }

            _db.Projects.Add(model);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Project created.";
            return RedirectToAction(nameof(Projects));
        }

        // GET: /Admin/ProjectDetails/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ProjectDetails(int id)
        {
            var project = await _db.Projects
                .Include(p => p.Assignments!)               
                    .ThenInclude(a => a.Employee)          
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null) return NotFound();

            project.Assignments = project.Assignments?
                                        .OrderByDescending(a => a.AssignedOn)
                                        .ToList();

            ViewBag.Employees = await _db.Employees.Where(e => e.IsActive).OrderBy(e => e.FullName).ToListAsync();

            return View("Projects/ProjectDetails", project);
        }

        // POST: /Admin/AssignToProject
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignToProject(int projectId, int employeeId, string? role, DateTime? startDate, DateTime? endDate)
        {
            var project = await _db.Projects.FindAsync(projectId);
            if (project == null) return NotFound();

            var employee = await _db.Employees.FindAsync(employeeId);
            if (employee == null) return NotFound();

            // avoid duplicate active assignment
            var exists = await _db.EmployeeProjects.AnyAsync(x => x.ProjectId == projectId && x.EmployeeId == employeeId && x.IsActive);
            if (!exists)
            {
                var ap = new EmployeeProject
                {
                    ProjectId = projectId,
                    EmployeeId = employeeId,
                    Role = role,
                    StartDate = startDate,
                    EndDate = endDate,
                    AssignedOn = DateTime.UtcNow,
                    IsActive = true
                };
                _db.EmployeeProjects.Add(ap);
                await _db.SaveChangesAsync();

                // Notification to the employee
                _db.Notifications.Add(new Notification
                {
                    EmployeeId = employeeId,
                    Title = $"Assigned to project: {project.Name}",
                    Message = $"You were assigned as '{role ?? "Member"}' to project '{project.Name}'. Deadline: {(project.Deadline?.ToShortDateString() ?? "N/A")}",
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false
                });
                await _db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(ProjectDetails), new { id = projectId });
        }

        // POST: /Admin/RemoveAssignment
        [HttpPost]
        public async Task<IActionResult> RemoveAssignment(int id)
        {
            var ap = await _db.EmployeeProjects
                              .Include(x => x.Employee)
                              .Include(x => x.Project)
                              .FirstOrDefaultAsync(x => x.Id == id);

            if (ap == null) return NotFound();

            // Create notification for the employee
            var note = new Notification
            {
                EmployeeId = ap.EmployeeId,
                Title = $"Removed from project: {ap.Project?.Name ?? "Project"}",
                Message = $"You have been removed from project '{ap.Project?.Name ?? "Project"}' by Admin.",
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };
            _db.Notifications.Add(note);
            _db.EmployeeProjects.Remove(ap);

            await _db.SaveChangesAsync();

            return Ok(new { success = true, message = "Assignment removed and employee notified." });
        }

        // GET: Admin/Assignments
        public async Task<IActionResult> Assignments()
        {
            var list = await _db.EmployeeProjects
                                .Include(a => a.Employee)
                                .Include(a => a.Project)
                                .OrderByDescending(a => a.AssignedOn)
                                .ToListAsync();
            return View("Projects/Assignments", list);
        }

        // Method to approve release
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveRelease(int assignmentId)
        {
            var ap = await _db.EmployeeProjects
                              .Include(x => x.Employee)
                              .Include(x => x.Project)
                              .FirstOrDefaultAsync(x => x.Id == assignmentId);

            if (ap == null) return NotFound();

            // Notifying employee before deletion
            var note = new Notification
            {
                EmployeeId = ap.EmployeeId,
                Title = $"Release approved: {ap.Project?.Name ?? "Project"}",
                Message = $"Your release request for project '{ap.Project?.Name ?? "Project"}' has been approved by the admin.",
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };
            _db.Notifications.Add(note);
            _db.EmployeeProjects.Remove(ap);

            await _db.SaveChangesAsync();

            TempData["Success"] = $"Release approved and {ap.Employee?.FullName} removed from '{ap.Project?.Name}'.";
            return RedirectToAction(nameof(ProjectDetails), new { id = ap.ProjectId });
        }

        // Method to deny release
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DenyRelease(int assignmentId)
        {
            var ap = await _db.EmployeeProjects
                              .Include(x => x.Employee)
                              .Include(x => x.Project)
                              .FirstOrDefaultAsync(x => x.Id == assignmentId);

            if (ap == null) return NotFound();

            ap.ReleaseRequested = false;
            ap.ReleaseComment = null;
            _db.EmployeeProjects.Update(ap);
            await _db.SaveChangesAsync();

            TempData["Success"] = $"Release request denied for {ap.Employee?.FullName}.";
            return RedirectToAction(nameof(ProjectDetails), new { id = ap.ProjectId });
        }
    }
}
