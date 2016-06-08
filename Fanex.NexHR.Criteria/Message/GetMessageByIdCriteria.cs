namespace Fanex.NexHR.Criteria.Message
{
    using Fanex.Data.Repository;

    public class GetMessageByIdCriteria : CriteriaBase
    {
        public int MessageId { get; set; }

        public override string GetSettingKey()
        {
            return "Message_GetById";
        }

        public override bool IsValid()
        {
            return MessageId > 0;
        }
    }
}
