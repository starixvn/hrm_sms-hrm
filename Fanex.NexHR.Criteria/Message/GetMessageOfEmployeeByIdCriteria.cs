namespace Fanex.NexHR.Criteria.Message
{
    using Fanex.Data.Repository;

    public class GetMessageOfEmployeeByIdCriteria : CriteriaBase
    {
        public int MessageId { get; set; }

        public override string GetSettingKey()
        {
            return "Message_GetDetailByEmployee";
        }

        public override bool IsValid()
        {
            return MessageId > 0;
        }
    }
}
