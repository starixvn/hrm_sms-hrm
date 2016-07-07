namespace Fanex.NexHR.Criteria.Memo
{
    using Fanex.Data.Repository;

    public class DeleteMemoCriteria : CriteriaBase
    {
        public int Id { get; set; }

        public override string GetSettingKey()
        {
            return "Memo_Delete";
        }

        public override bool IsValid()
        {
            return Id > 0;
        }
    }
}
