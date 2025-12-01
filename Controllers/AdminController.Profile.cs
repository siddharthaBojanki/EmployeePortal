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
        // GET: Admin/Profile
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
    }
}
