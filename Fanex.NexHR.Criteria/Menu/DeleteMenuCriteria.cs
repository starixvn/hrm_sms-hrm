namespace Fanex.NexHR.Criteria.Menu
{
    using Fanex.Data.Repository;

    public class DeleteMenuCriteria : CriteriaBase
    {
        public int MenuId { get; set; }

        public override string GetSettingKey()
        {
            return "Menu_Delete";
        }

        public override bool IsValid()
        {
            return MenuId > 0;
        }
    }
}
