namespace Fanex.NexHR.Models
{
    public class EmployeeLeaveBalance
    {
        public int EmployeeId { get; set; }

        public decimal AnnualBalance { get; set; }

        public decimal KeepInViewBalance { get; set; }

        public decimal CurrentLeaveBalanceDay { get; set; }
    }
}
