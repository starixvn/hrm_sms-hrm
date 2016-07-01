namespace Fanex.NexHR.Criteria.AnnualLeave
{
    using System;
    using Fanex.Data.Repository;

    public class AdjustEmployeeAnnualLeaveCriteria : CriteriaBase
    {
        public string EmpIds { get; set; }

        public string Description { get; set; }

        public DateTime FromDate { get; set; }

        public decimal DeductDays { get; set; }

        public decimal CreditDays { get; set; }

        public DateTime CreatedDate { get; set; }

        public int CreatedBy { get; set; }

        public override string GetSettingKey()
        {
            return "AnnualLeave_InsertMultipleTransactions";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(EmpIds);
        }
    }
}
