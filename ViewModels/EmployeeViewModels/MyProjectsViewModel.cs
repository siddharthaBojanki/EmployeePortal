using System;
using System.Collections.Generic;

namespace EmployeePortal.ViewModels.EmployeeViewModels
{
    public class MyProjectsViewModel
    {
        public int EmployeeId { get; set; }
        public string FullName { get; set; } = "";

        public List<ProjectAssignmentDto> Assignments { get; set; } = new();

        public class ProjectAssignmentDto
        {
            public int AssignmentId { get; set; }
            public int ProjectId { get; set; }
            public string ProjectName { get; set; } = "";
            public string? Client { get; set; }
            public string? Role { get; set; }
            public DateTime AssignedOn { get; set; }
            public DateTime? ProjectDeadline { get; set; }
            public string? ProjectDescription { get; set; }
            public bool IsActive { get; set; }
        }
    }
}
