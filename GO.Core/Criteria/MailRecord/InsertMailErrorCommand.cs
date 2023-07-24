namespace GO.Core.Criteria.MailRecord
{
    using GO.Core.Models;

    public class InsertMailErrorCommand : BaseNonQueryCommand
    {
        public string MailTo { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }

        public override string GetSettingKey() => "MailError_Insert";

        public override bool IsValid() => true;
    }
}