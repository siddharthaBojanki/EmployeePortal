using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EmployeePortal.Models
{
    public enum ProjectStatus
    {
        Planned,
        Active,
        Completed,
        OnHold,
        Cancelled
    }

    public class Project
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; } = "";

        [MaxLength(200)]
        public string? Client { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? Deadline { get; set; }   

        [MaxLength(1000)]
        public string? Description { get; set; }

        public ProjectStatus Status { get; set; } = ProjectStatus.Planned;

        public ICollection<EmployeeProject>? Assignments { get; set; }
    }
}
