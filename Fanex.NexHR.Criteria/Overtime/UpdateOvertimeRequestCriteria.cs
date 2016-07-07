namespace Fanex.NexHR.Criteria.Overtime
{
    using System.Collections.Generic;
    using System.Text;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;

    public class UpdateOvertimeRequestCriteria : CriteriaBase
    {
        private const string _OverTimeRequesttDetailTpl = "<Request OnDate='{0}' ShiftID='{1}' TimeIn='{2}' "
            + "TimeOut='{3}' TotalHours='{4}' NormalHours='{5}' OffHours='{6}' PublicHours='{7}' Remark='{8}'></Request>";

        [SpParam(SqlParamName = "EmpId")]
        public int EmployeeId { get; set; }

        public short RequestType { get; set; }

        public string Remark { get; set; }

        public int ProcessedBy { get; set; }

        public int? VerifierId { get; set; }

        public int? ApproverId { get; set; }

        public int? HRId { get; set; }

        public IList<OvertimeDaily> OvertimeRequest { get; set; }

        public string ListDetail
        {
            get
            {
                var value = new StringBuilder("<Requests>");

                if (OvertimeRequest != null)
                {
                    foreach (var detail in OvertimeRequest)
                    {
                        value.AppendFormat(
                            _OverTimeRequesttDetailTpl,
                            detail.OvertimeDate.ToString("yyyyMMdd"),
                            detail.Schedule,
                            detail.TimeIn != null ? detail.TimeIn : string.Empty,
                            detail.TimeOut != null ? detail.TimeOut : string.Empty,
                            detail.TotalWorkHours,
                            detail.NormalHours,
                            detail.OffHours,
                            detail.PublicHours,
                            detail.Remark);
                    }
                }

                return value.Append("</Requests>").ToString();
            }
        }

        public bool IsDebug { get; set; }

        public override string GetSettingKey()
        {
            return "Overtime_SET_OTRequest";
        }

        public override bool IsValid()
        {
            return EmployeeId > 0;
        }
    }
}