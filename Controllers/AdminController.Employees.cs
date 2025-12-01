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
        // GET: Admin/Index   - All employees
        public async Task<IActionResult> Index()
        {
            var list = await _db.Employees.Include(e => e.Department)
                                          .Include(e => e.User)
                                          .OrderBy(e => e.FullName)
                                          .ToListAsync();
            return View("Employees/Index", list);
        }

        // GET: Admin/Create
        public IActionResult Create()
        {
            ViewBag.Departments = _db.Departments.OrderBy(d => d.Name).ToList();
            return View("Employees/Create", new CreateEmployeeModel { DateOfJoining = DateTime.UtcNow.Date });
        }

        // POST: Admin/Index
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateEmployeeModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Departments = _db.Departments.OrderBy(d => d.Name).ToList();
                return View("Employees/Create", model);
            }

            var user = new ApplicationUser { UserName = model.Email, Email = model.Email, EmailConfirmed = true };
            var createUser = await _userManager.CreateAsync(user, model.Password);
            if (!createUser.Succeeded)
            {
                foreach (var e in createUser.Errors) ModelState.AddModelError("", e.Description);
                ViewBag.Departments = _db.Departments.OrderBy(d => d.Name).ToList();
                return View("Employees/Create", model);
            }

            if (!await _roleManager.RoleExistsAsync("Employee"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Employee"));
            }

            await _userManager.AddToRoleAsync(user, "Employee");

            var emp = new Employee
            {
                UserId = user.Id,
                FullName = model.FullName,
                Phone = model.Phone,
                Address = model.Address,
                DepartmentId = model.DepartmentId,
                DateOfJoining = model.DateOfJoining,
                Role = RoleType.Employee,
                IsActive = true
            };

            _db.Employees.Add(emp);
            await _db.SaveChangesAsync();

            var notif = new Notification
            {
                EmployeeId = emp.Id,
                Title = "Welcome to the company",
                Message = $"Hello {emp.FullName}, your account has been created.\n" +
                            $"Temporary password: {model.Password}. Please change your password on first login.",
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            _db.Notifications.Add(notif);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Employee created.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/Edit
        public async Task<IActionResult> Edit(int id)
        {
            var emp = await _db.Employees.FindAsync(id);
            if (emp == null) return NotFound();

            ViewBag.Departments = _db.Departments.OrderBy(d => d.Name).ToList();

            var vm = new EditEmployeeModel
            {
                Id = emp.Id,
                FullName = emp.FullName,
                Phone = emp.Phone,
                Address = emp.Address,
                DepartmentId = emp.DepartmentId,
                DateOfJoining = emp.DateOfJoining,
                IsActive = emp.IsActive
            };
            return View("Employees/Edit", vm);
        }

        // POST: Admin/Edit
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditEmployeeModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Departments = _db.Departments.OrderBy(d => d.Name).ToList();
                return View("Employees/Edit", model);
            }

            var emp = await _db.Employees.FindAsync(model.Id);
            if (emp == null) return NotFound();

            emp.FullName = model.FullName;
            emp.Phone = model.Phone;
            emp.Address = model.Address;
            emp.DepartmentId = model.DepartmentId;
            emp.DateOfJoining = model.DateOfJoining;
            emp.IsActive = model.IsActive;

            _db.Employees.Update(emp);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Employee updated.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var emp = await _db.Employees
                .Include(e => e.Department)
                .Include(e => e.User)
                .Include(e => e.LeaveRequests)
                .Include(e => e.WfhRequests)
                .Include(e => e.PerformanceReviews)
                .Include(e => e.Notifications)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (emp == null) return NotFound();

            return View("Employees/Details", emp);
        }

        // GET: Admin/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var emp = await _db.Employees.Include(e => e.User).FirstOrDefaultAsync(e => e.Id == id);
            if (emp == null) return NotFound();
            return View("Employees/Delete", emp);
        }

        // POST: Admin/Delete/5
        [HttpPost, ActionName("DeleteConfirmed"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var emp = await _db.Employees.FindAsync(id);
            if (emp == null) return NotFound();

            var userId = emp.UserId;

            _db.Employees.Remove(emp);
            await _db.SaveChangesAsync();

            if (!string.IsNullOrEmpty(userId))
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    await _userManager.DeleteAsync(user);
                }
            }

            TempData["Success"] = "Employee deleted.";
            return RedirectToAction(nameof(Index));
        }
    }
}
