namespace Fanex.NexHR.Criteria.Menu
{
    using Fanex.Data.Repository;

    public class GetMenusCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "Menu_GetMenus";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
