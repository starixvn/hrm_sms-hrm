namespace Fanex.NexHR.Criteria.Message
{
    using Fanex.Data.Repository;

    public class UpdateReadStatusMessageCriteria : CriteriaBase
    {
        public int MessageId { get; set; }

        public int EmployeeId { get; set; }

        public override string GetSettingKey()
        {
            return "Message_Read";
        }

        public override bool IsValid()
        {
            return MessageId > 0 && EmployeeId > 0;
        }
    }
}
