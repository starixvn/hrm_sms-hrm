namespace Fanex.NexHR.Criteria.Request.Leave
{
    public class InsertLeaveRequestCriteria : BaseLeaveRequestCriteria
    {
        public int CreatedBy { get; set; }

        public override string GetSettingKey()
        {
            return "WorkingTimeRequest_Insert_LeaveMultiRequest";
        }

        public override bool IsValid()
        {
            return EmployeeId > 0 && FromDate <= ToDate;
        }
    }
}