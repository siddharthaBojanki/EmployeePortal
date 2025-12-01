using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmployeePortal.Data;
using EmployeePortal.Models;

namespace EmployeePortal.Controllers
{
    public partial class AdminController : Controller
    {
        // GET: Admin/Departments
        public async Task<IActionResult> Departments()
        {
            var list = await _db.Departments.OrderBy(d => d.Name).ToListAsync();
            return View("Departments/Index", list);
        }

        // GET: Admin/CreateDepartment
        public IActionResult CreateDepartment()
        {
            return View("Departments/Create", new Department());
        }

        // POST: Admin/CreateDepartment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDepartment(Department model)
        {
            if (!ModelState.IsValid)
                return View("Departments/Create", model);

            _db.Departments.Add(model);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Department created.";
            return RedirectToAction(nameof(Departments));
        }

        // GET: Admin/EditDepartment/5
        public async Task<IActionResult> EditDepartment(int id)
        {
            var dept = await _db.Departments.FindAsync(id);
            if (dept == null) return NotFound();
            return View("Departments/Edit", dept);
        }

        // POST: Admin/EditDepartment/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDepartment(Department model)
        {
            if (!ModelState.IsValid)
                return View("Departments/Edit", model);

            var dept = await _db.Departments.FindAsync(model.Id);
            if (dept == null) return NotFound();

            dept.Name = model.Name;

            _db.Departments.Update(dept);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Department updated.";
            return RedirectToAction(nameof(Departments));
        }

        // GET: Admin/DeleteDepartment/5
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            var dept = await _db.Departments.FindAsync(id);
            if (dept == null) return NotFound();
            return View("Departments/Delete", dept);
        }

        // Method to confirm deletion of department
        [HttpPost, ActionName("DeleteConfirmedDepartment")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmedDepartment(int id)
        {
            var dept = await _db.Departments.FindAsync(id);
            if (dept == null) return NotFound();

            var hasEmployees = await _db.Employees.AnyAsync(e => e.DepartmentId == id);
            if (hasEmployees)
            {
                TempData["Error"] = "Cannot delete department that has employees assigned. Reassign or remove employees first.";
                return RedirectToAction(nameof(Departments));
            }

            _db.Departments.Remove(dept);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Department deleted.";
            return RedirectToAction(nameof(Departments));
        }
    }
}
