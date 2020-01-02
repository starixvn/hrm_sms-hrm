namespace Fanex.NexHR.Criteria.Section
{
    using Fanex.Data.Repository;

    public class EditSectionCriteria : CriteriaBase
    {
        public int GroupId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public int DepartmentId { get; set; }

        public string MailGroup { get; set; }

        public string LeaderIds { get; set; }

        public string AssistantLeaderIds { get; set; }
        public byte BasicAnnualLeaveBalance { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "Section_Update";
        }

        public override bool IsValid()
        {
            return GroupId > 0 &&
                !string.IsNullOrWhiteSpace(Code) &&
                !string.IsNullOrWhiteSpace(Name) &&
                !string.IsNullOrWhiteSpace(LeaderIds);
        }
    }
}