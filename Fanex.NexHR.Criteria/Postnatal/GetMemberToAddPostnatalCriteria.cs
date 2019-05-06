namespace Fanex.NexHR.Criteria.Postnatal
{
    using Fanex.Data.Repository;

    public class GetMemberToAddPostnatalCriteria : CriteriaBase
    {
        public string Query { get; set; }

        public int SupervisorId { get; set; }

        public override bool IsValid()
        {
            return SupervisorId > 0;
        }

        public override string GetSettingKey()
        {
            return "Postnatal_GetMember";
        }
    }
}