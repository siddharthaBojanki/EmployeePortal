using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;

namespace EmployeePortal.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string FullName { get; set; } = "";

        [MaxLength(15)]
        public string? Phone { get; set; }

        [MaxLength(250)]
        public string? Address { get; set; }

        public DateTime DateOfJoining { get; set; }

        public RoleType Role { get; set; } = RoleType.Employee;
        public bool IsActive { get; set; } = true;

        public int? DepartmentId { get; set; }
        public Department? Department { get; set; }

        public ICollection<LeaveRequest>? LeaveRequests { get; set; }
        public ICollection<WfhRequest>? WfhRequests { get; set; }
        public ICollection<PerformanceReview>? PerformanceReviews { get; set; }
        public ICollection<Notification>? Notifications { get; set; }

        // link to AspNetUsers.Id (string)
        [MaxLength(450)]
        public string? UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser? User { get; set; }
    }
}
