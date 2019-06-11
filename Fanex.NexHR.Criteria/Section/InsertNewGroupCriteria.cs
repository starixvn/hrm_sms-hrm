namespace Fanex.NexHR.Criteria.Team
{
    using System;
    using Fanex.Data.Repository;

    public class InsertNewGroupCriteria : CriteriaBase
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public int DepartmentId { get; set; }

        public string MailGroup { get; set; }

        public int LeaderId { get; set; }

        public int? AssistantLeaderId { get; set; }

        public int CreatedBy { get; set; }

        public override string GetSettingKey()
        {
            return "EmployeeGroup_Insert";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Code) && !string.IsNullOrWhiteSpace(Name);
        }
    }
}