namespace Fanex.NexHR.Criteria.Request.ChangeShift
{
    public class UpdateChangeShiftRequestCriteria : ChangeShiftCommand
    {
        public long RequestId { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "WorkingTimeRequest_Update_ChangeShiftRequest";
        }

        public override bool IsValid()
        {
            return RequestId > 0 && EmpId > 0 && FromDate <= ToDate;
        }
    }
}
