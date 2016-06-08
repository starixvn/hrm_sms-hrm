namespace Fanex.NexHR.Criteria.Message
{
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;
    using System;

    public class InsertMessageCriteria : CriteriaBase
    {
        public InsertMessageCriteria(Message message)
        {
            if (message != null)
            {
                Subject = message.Subject;
                Contents = message.Contents;
                Category = message.Category;
                FromDate = message.FromDate;
                ToDate = message.ToDate;
                EmployeeIds = message.EmployeeIds;
                GroupIds = message.GroupIds;
            }
        }

        public string Subject { get; set; }

        public string Contents { get; set; }

        public int Category { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public string EmployeeIds { get; set; }

        public string GroupIds { get; set; }

        public int CreatedBy { get; set; }

        public override string GetSettingKey()
        {
            return "Message_Insert";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
