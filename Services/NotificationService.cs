using EmployeePortal.Models;
using Microsoft.Extensions.Logging;
using EmployeePortal.Data;
using System;
using System.Threading.Tasks;

namespace EmployeePortal.Services
{
    public class NotificationService
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(ApplicationDbContext db, ILogger<NotificationService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<Notification?> CreateNotification(int employeeId, string title, string message)
        {
            try
            {
                var n = new Notification
                {
                    EmployeeId = employeeId,
                    Title = title,
                    Message = message,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                };

                _db.Notifications.Add(n);
                await _db.SaveChangesAsync();
                return n;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create notification for Employee {EmployeeId}", employeeId);
                return null;
            }
        }
    }
}
