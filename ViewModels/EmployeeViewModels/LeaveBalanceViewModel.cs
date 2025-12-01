namespace EmployeePortal.ViewModels.EmployeeViewModels
{
    public class LeaveBalanceViewModel
    {
        public int CasualLeaves { get; set; }
        public int SickLeaves { get; set; }
        public int OtherLeaves { get; set; }

        public int CasualUsed { get; set; }
        public int SickUsed { get; set; }
        public int OtherUsed { get; set; }

        public int CasualRemaining => CasualLeaves - CasualUsed;
        public int SickRemaining => SickLeaves - SickUsed;
        public int OtherRemaining => OtherLeaves - OtherUsed;
    }
}
