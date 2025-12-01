using System;
using System.Threading.Tasks;
using EmployeePortal.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeePortal.Data
{
    public static class SeedSampleData
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var db = services.GetRequiredService<ApplicationDbContext>();

            try
            {
                // Ensure roles
                if (!await roleManager.RoleExistsAsync("Admin"))
                    await roleManager.CreateAsync(new IdentityRole("Admin"));
                if (!await roleManager.RoleExistsAsync("Employee"))
                    await roleManager.CreateAsync(new IdentityRole("Employee"));

                // ---------- Admin ----------
                var adminEmail = "admin@company.com";
                var adminPassword = "Admin@123"; // change for production

                var adminUser = await userManager.FindByEmailAsync(adminEmail);
                if (adminUser == null)
                {
                    adminUser = new ApplicationUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        EmailConfirmed = true
                    };
                    var result = await userManager.CreateAsync(adminUser, adminPassword);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(adminUser, "Admin");

                        // Create Employee record for admin
                        db.Employees.Add(new Employee
                        {
                            UserId = adminUser.Id,
                            FullName = "System Administrator",
                            Phone = "9999999999",
                            Address = "Head Office",
                            DateOfJoining = DateTime.UtcNow,
                            Role = RoleType.Admin,
                            IsActive = true
                        });
                        await db.SaveChangesAsync();
                    }
                    else
                    {
                        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                        Console.WriteLine($"Failed to create admin user: {errors}");
                    }
                }
                else
                {
                    // Ensure admin role
                    if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
                        await userManager.AddToRoleAsync(adminUser, "Admin");

                    // Ensure Employee record exists
                    if (!await db.Employees.AnyAsync(e => e.UserId == adminUser.Id))
                    {
                        db.Employees.Add(new Employee
                        {
                            UserId = adminUser.Id,
                            FullName = "System Administrator",
                            Phone = "9999999999",
                            Address = "Head Office",
                            DateOfJoining = DateTime.UtcNow,
                            Role = RoleType.Admin,
                            IsActive = true
                        });
                        await db.SaveChangesAsync();
                    }
                }

                // ---------- Employee ----------
                var empEmail = "employee1@company.com";
                var empPassword = "Emp@12345"; // change for production

                var empUser = await userManager.FindByEmailAsync(empEmail);
                if (empUser == null)
                {
                    empUser = new ApplicationUser
                    {
                        UserName = empEmail,
                        Email = empEmail,
                        EmailConfirmed = true
                    };
                    var result = await userManager.CreateAsync(empUser, empPassword);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(empUser, "Employee");

                        // Create Employee record
                        db.Employees.Add(new Employee
                        {
                            UserId = empUser.Id,
                            FullName = "John Employee",
                            Phone = "8888888888",
                            Address = "Hyderabad",
                            DateOfJoining = DateTime.UtcNow.AddDays(-7),
                            Role = RoleType.Employee,
                            IsActive = true
                        });
                        await db.SaveChangesAsync();
                    }
                    else
                    {
                        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                        Console.WriteLine($"Failed to create employee user: {errors}");
                    }
                }
                else
                {
                    // Ensure employee role
                    if (!await userManager.IsInRoleAsync(empUser, "Employee"))
                        await userManager.AddToRoleAsync(empUser, "Employee");

                    // Ensure Employee record exists
                    if (!await db.Employees.AnyAsync(e => e.UserId == empUser.Id))
                    {
                        db.Employees.Add(new Employee
                        {
                            UserId = empUser.Id,
                            FullName = "John Employee",
                            Phone = "8888888888",
                            Address = "Hyderabad",
                            DateOfJoining = DateTime.UtcNow.AddDays(-7),
                            Role = RoleType.Employee,
                            IsActive = true
                        });
                        await db.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during seeding: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                // Don't throw - allow app to continue
            }
        }
    }
}