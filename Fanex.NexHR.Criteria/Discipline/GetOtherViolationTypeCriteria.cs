namespace Fanex.NexHR.Criteria.Discipline
{
    using Fanex.Data.Repository;

    public class GetOtherViolationTypeCriteria : CriteriaBase
    {
        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "Discipline_GetOtherViolationType";
        }
    }
}