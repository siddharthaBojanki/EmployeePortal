using EmployeePortal.Data;
using EmployeePortal.Models;
using EmployeePortal.ViewModels.EmployeeViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MyApp.Services
{
    public class LeaveBalanceService
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<LeaveBalanceService> _logger;

        public LeaveBalanceService(ApplicationDbContext db, ILogger<LeaveBalanceService> logger)
        {
            _db = db;
            _logger = logger;
        }

        //Maximum leaves per year
        private readonly int CasualLeaves = 12;
        private readonly int SickLeaves = 8;
        private readonly int OtherLeaves = 5;

        public async Task<LeaveBalanceViewModel> CalculateLeaveBalance(int employeeId)
        {
            try
            {
                // yearly limits (you can move these to config or database later)
                int casual = CasualLeaves;
                int sick = SickLeaves;
                int other = OtherLeaves;

                var yearStart = new DateTime(DateTime.UtcNow.Year, 1, 1);
                var yearEnd = yearStart.AddYears(1).AddDays(-1);

                // get all approved leaves for the year
                var used = await _db.LeaveRequests
                    .Where(l => l.EmployeeId == employeeId
                                && l.Status == RequestStatus.Approved
                                && l.FromDate <= yearEnd
                                && l.ToDate >= yearStart)
                    .ToListAsync();

                // calculate days taken per type
                int usedCasual = used.Where(u => u.LeaveType == LeaveType.Casual)
                                     .Sum(u => (int)((u.ToDate.Date - u.FromDate.Date).TotalDays + 1));

                int usedSick = used.Where(u => u.LeaveType == LeaveType.Sick)
                                     .Sum(u => (int)((u.ToDate.Date - u.FromDate.Date).TotalDays + 1));

                int usedOther = used.Where(u => u.LeaveType == LeaveType.Other)
                                     .Sum(u => (int)((u.ToDate.Date - u.FromDate.Date).TotalDays + 1));

                return new LeaveBalanceViewModel
                {
                    CasualLeaves = casual,
                    SickLeaves = sick,
                    OtherLeaves = other,
                    CasualUsed = usedCasual,
                    SickUsed = usedSick,
                    OtherUsed = usedOther
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to calculate leave balance for Employee {EmployeeId}", employeeId);
                return new LeaveBalanceViewModel(); // safe fallback
            }
        }
    }
}
