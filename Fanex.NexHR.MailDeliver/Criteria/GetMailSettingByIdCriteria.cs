namespace Fanex.NexHR.MailDeliver.Criteria
{
    using Data.Repository;

    public class GetMailSettingByIdCriteria : CriteriaBase
    { 
        public int SettingId { get; set; }

        public override string GetSettingKey()
        {
            return "MailRecorder_GetSetting";
        }

        public override bool IsValid()
        {
            return SettingId > 0;
        }
    }
}
