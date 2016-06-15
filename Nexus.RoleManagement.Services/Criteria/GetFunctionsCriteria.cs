namespace Nexus.RoleManagement.Services.Services.Implement
{
    using Fanex.Data.Repository;

    public class GetFunctionsCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "Function_GetFunctions";
        }
    }
}