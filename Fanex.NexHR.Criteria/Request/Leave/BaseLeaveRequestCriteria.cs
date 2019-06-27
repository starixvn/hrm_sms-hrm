namespace Fanex.NexHR.Criteria.Request.Leave
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Core.Attributes;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Models.Request;

    public class BaseLeaveRequestCriteria : CriteriaBase
    {
        private const string LeaveRequestDetailTpl =
            "<LeaveRequestDetail RequestId='{0}' EmpId='{1}' OnDate='{2}' "
            + "LeaveType='{3}' IsHalfDay='{4}' WorkType='{5}' FromTime='{6}' ToTime='{7}'></LeaveRequestDetail>";

        [Output]
        public long RequestId { get; set; }

        [SpParam(SqlParamName = "EmpId")]
        public int EmployeeId { get; set; }

        [SpParam(Ignored = true)]
        public string EmployeeName { get; set; }

        public string Remark { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int LeaveTypeId { get; set; }

        [SpParam(Ignored = true)]
        public string LeaveTypeName { get; set; }

        public decimal TotalDays { get; set; }

        public int? VerifierId { get; set; }

        public int? ApproverId { get; set; }

        public string RefNo { get; set; }

        [SpParam(Ignored = true)]
        public IList<LeaveDetailModel> RequestDetail { get; set; }

        public string ListDetail
        {
            get
            {
                var value = new StringBuilder("<ROOT>");

                if (RequestDetail != null)
                {
                    foreach (var detail in RequestDetail)
                    {
                        value.AppendFormat(
                            LeaveRequestDetailTpl,
                            0,
                            EmployeeId,
                            detail.OnDate,
                            detail.WorkType == (int)WorkType.Leave ? detail.LeaveTypeId.ToString() : string.Empty,
                            detail.IsHalfDay,
                            detail.WorkType,
                            detail.FromTime,
                            detail.ToTime);
                    }
                }

                return value.Append("</ROOT>").ToString();
            }
        }
    }
}