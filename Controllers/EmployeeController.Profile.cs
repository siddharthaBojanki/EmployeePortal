using EmployeePortal.ViewModels.EmployeeViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeePortal.Controllers
{
    public partial class EmployeeController : Controller
    {
        // GET: Employee/Profile
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();
            var emp = await _db.Employees.Include(e => e.Department)
                                         .FirstOrDefaultAsync(e => e.UserId == user.Id);
            if (emp == null) return NotFound();

            var vm = new ProfileViewModel
            {
                Employee = emp,
                Email = user.Email
            };
            return View("Profile/Profile", emp);
        }

        // GET: Employee/EditContact
        [HttpGet]
        public async Task<IActionResult> EditContact()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();
            var emp = await _db.Employees.FirstOrDefaultAsync(e => e.UserId == user.Id);
            if (emp == null) return NotFound();

            var vm = new EditContactModel
            {
                Phone = emp.Phone,
                Address = emp.Address
            };
            return View("Profile/EditContact", vm);
        }

        // POST: Employee/EditContact
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditContact(EditContactModel model)
        {
            if (!ModelState.IsValid)
                return View("Profile/EditContact", model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();
            var emp = await _db.Employees.FirstOrDefaultAsync(e => e.UserId == user.Id);
            if (emp == null) return NotFound();

            emp.Phone = model.Phone;
            emp.Address = model.Address;
            _db.Employees.Update(emp);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Contact information updated.";
            return RedirectToAction(nameof(Profile));
        }
    }
}
