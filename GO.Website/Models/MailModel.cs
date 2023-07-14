namespace GO.Website.Models
{
    public class MailModel
    {
        public short? PeriodId { get; set; }
        public int? UserId { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string MailTo { get; set; }
    }
}