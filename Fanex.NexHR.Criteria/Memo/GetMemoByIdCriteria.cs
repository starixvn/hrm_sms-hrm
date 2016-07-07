namespace Fanex.NexHR.Criteria.Memo
{
    using Fanex.Data.Repository;

    public class GetMemoByIdCriteria : CriteriaBase
    {
        public int Id { get; set; }

        public override string GetSettingKey()
        {
            return "Memo_GetById";
        }

        public override bool IsValid()
        {
            return Id > 0;
        }
    }
}
