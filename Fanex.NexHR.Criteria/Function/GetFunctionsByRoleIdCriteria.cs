namespace Fanex.NexHR.Criteria.Function
{
    using Fanex.Data.Repository;

    public class GetFunctionsByRoleIdCriteria : CriteriaBase
    {
        public int RoleId { get; set; }

        public override bool IsValid()
        {
            return RoleId > 0;
        }

        public override string GetSettingKey()
        {
            return "Function_GetFunctionsByRoleId";
        }
    }
}
