using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EmployeePortal.Models;

namespace EmployeePortal.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Employee> Employees { get; set; } = null!;
        public DbSet<Department> Departments { get; set; } = null!;
        public DbSet<LeaveRequest> LeaveRequests { get; set; } = null!;
        public DbSet<WfhRequest> WfhRequests { get; set; } = null!;
        public DbSet<PerformanceReview> PerformanceReviews { get; set; } = null!;
        public DbSet<Notification> Notifications { get; set; } = null!;
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<EmployeeProject> EmployeeProjects { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Employee.UserId unique -> one-to-one with AspNetUsers
            builder.Entity<Employee>()
                .HasIndex(e => e.UserId)
                .IsUnique();

            builder.Entity<Employee>()
                .HasOne(e => e.User)
                .WithOne(u => u.Employee)
                .HasForeignKey<Employee>(e => e.UserId)
                .HasPrincipalKey<ApplicationUser>(u => u.Id)
                .OnDelete(DeleteBehavior.Cascade);

            // Employee -> Department (many-to-one)
            builder.Entity<Employee>()
                .HasOne(e => e.Department)
                .WithMany(d => d.Employees)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.SetNull);

            // LeaveRequest -> Employee
            builder.Entity<LeaveRequest>()
                .HasOne(l => l.Employee)
                .WithMany(e => e.LeaveRequests)
                .HasForeignKey(l => l.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            // WfhRequest -> Employee
            builder.Entity<WfhRequest>()
                .HasOne(w => w.Employee)
                .WithMany(e => e.WfhRequests)
                .HasForeignKey(w => w.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            // PerformanceReview relations
            builder.Entity<PerformanceReview>()
                .HasOne(pr => pr.Employee)
                .WithMany(e => e.PerformanceReviews)
                .HasForeignKey(pr => pr.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<PerformanceReview>()
                .HasOne(pr => pr.ReviewedByEmployee)
                .WithMany()
                .HasForeignKey(pr => pr.ReviewedBy)
                .OnDelete(DeleteBehavior.SetNull);

            // Notification -> Employee
            builder.Entity<Notification>()
                .HasOne(n => n.Employee)
                .WithMany(e => e.Notifications)
                .HasForeignKey(n => n.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes and uniqueness
            builder.Entity<Employee>()
                .HasIndex(e => e.FullName);
        }
    }
}

