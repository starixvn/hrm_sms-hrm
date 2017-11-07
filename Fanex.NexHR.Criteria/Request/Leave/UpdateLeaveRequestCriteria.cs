namespace Fanex.NexHR.Criteria.Request.Leave
{
    public class UpdateLeaveRequestCriteria : BaseLeaveRequestCriteria
    {
        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "WorkingTimeRequest_Update_LeaveMultiRequest";
        }

        public override bool IsValid()
        {
            return RequestId > 0 && EmployeeId > 0 && FromDate <= ToDate;
        }
    }
}
