namespace Fanex.NexHR.Criteria.Message
{
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;
    using System;

    public class UpdateMessageCriteria : CriteriaBase
    {
        public UpdateMessageCriteria(Message message)
        {
            if (message != null)
            {
                MessageId = message.MessageId;
                Subject = message.Subject;
                Contents = message.Contents;
                Category = message.Category;
                FromDate = message.FromDate;
                ToDate = message.ToDate;
                EmployeeIds = message.EmployeeIds;
                GroupIds = message.GroupIds;
            }
        }

        public int MessageId { get; set; }

        public string Subject { get; set; }

        public string Contents { get; set; }

        public int Category { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public string EmployeeIds { get; set; }

        public string GroupIds { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "Message_Update";
        }

        public override bool IsValid()
        {
            return MessageId > 0;
        }
    }
}
