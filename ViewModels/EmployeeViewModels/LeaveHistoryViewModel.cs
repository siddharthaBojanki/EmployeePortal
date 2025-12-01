using System.Collections.Generic;
using EmployeePortal.Models;

namespace EmployeePortal.ViewModels.EmployeeViewModels
{
    public class LeaveHistoryViewModel
    {
        public IEnumerable<LeaveRequest>? Leaves { get; set; }
        public LeaveBalanceViewModel? LeaveBalance { get; set; }
    }
}
