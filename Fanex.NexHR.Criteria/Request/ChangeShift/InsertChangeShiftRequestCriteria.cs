namespace Fanex.NexHR.Criteria.Request.ChangeShift
{
    using Fanex.NexHR.Core.Attributes;

    public class InsertChangeShiftRequestCriteria : ChangeShiftCommand
    {
        [Output]
        public long RequestId { get; set; }

        public int CreatedBy { get; set; }

        public override string GetSettingKey()
        {
            return "WorkingTimeRequest_Insert_ChangeShift";
        }

        public override bool IsValid()
        {
            return EmpId > 0 && FromDate <= ToDate;
        }
    }
}
