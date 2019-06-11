namespace Fanex.NexHR.Criteria.Team
{
    using System;
    using Fanex.Data.Repository;

    public class UpdateGroupCriteria : CriteriaBase
    {
        public int GroupId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public int DepartmentId { get; set; }

        public string MailGroup { get; set; }

        public int LeaderId { get; set; }

        public int? AssistantLeaderId { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "EmployeeGroup_Update";
        }

        public override bool IsValid()
        {
            return GroupId > 0 && !string.IsNullOrWhiteSpace(Code) && !string.IsNullOrWhiteSpace(Name);
        }
    }
}
