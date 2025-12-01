using Microsoft.AspNetCore.Identity;

namespace EmployeePortal.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Navigation to Employees table (one-to-one)
        public virtual Employee? Employee { get; set; }
    }
}
