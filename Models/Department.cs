using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EmployeePortal.Models
{
    public class Department
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public virtual ICollection<Employee>? Employees { get; set; }
    }
}
