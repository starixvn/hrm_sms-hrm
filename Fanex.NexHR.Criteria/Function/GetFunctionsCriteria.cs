namespace Fanex.NexHR.Criteria.Function
{
    using Fanex.Data.Repository;

    public class GetFunctionsCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "Function_GetFunctions";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}