namespace Fanex.NexHR.Criteria.Group
{
    using Fanex.Data.Repository;

    public class CreateTeamCriteria : CriteriaBase
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public int SectionId { get; set; }

        public string MailGroup { get; set; }

        public int HeadId { get; set; }
        public string ManagerIds { get; set; }
        public string LeaderIds { get; set; }
        public string SupervisorIds { get; set; }
        public byte BasicAnnualLeaveBalance { get; set; }
        public byte WorkingDays { get; set; }
        public byte WorkingHours { get; set; }
        public byte OffDaysbyWeek { get; set; }

        public int CreatedBy { get; set; }

        public override string GetSettingKey()
        {
            return "Group_Insert";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Code)
                && !string.IsNullOrWhiteSpace(Name)
                && HeadId > 0;
        }
    }
}