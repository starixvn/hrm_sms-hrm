namespace GO.Core.Criteria.MailRecord
{
    using GO.Core.Models;

    public class InsertMailRecordCommand : BaseNonQueryCommand
    {
        public string MailTo { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

        public override string GetSettingKey() => "MailRecord_Insert";

        public override bool IsValid() => true;
    }
}