namespace Fanex.NexHR.Criteria.AnnualLeave
{
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class ResetAnnualLeaveCriteria : CriteriaBase
    {
        private const string _AnnualLeaveTpl = "<Employee Id=\"{0}\" LeaveBalance=\"{1}\"/>";

        [SpParam(Ignored = true)]
        public IList<LeaveSummary> AnnualLeaves { get; set; }

        public string EmpValue
        {
            get
            {
                var value = new StringBuilder("<ROOT>");

                if (AnnualLeaves != null)
                {
                    foreach (LeaveSummary leaveSummary in AnnualLeaves)
                    {
                        value.AppendFormat(_AnnualLeaveTpl, leaveSummary.Employee.Id, leaveSummary.LeaveBalance);
                    }
                }

                return value.Append("</ROOT>").ToString();
            }
        }

        public string Description { get; set; }

        public DateTime FromDate { get; set; }

        public decimal ResetDays { get; set; }

        public DateTime CreatedDate { get; set; }

        public int CreatedBy { get; set; }

        public override string GetSettingKey()
        {
            return "AnnualLeave_ResetMultipleTransactions";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
