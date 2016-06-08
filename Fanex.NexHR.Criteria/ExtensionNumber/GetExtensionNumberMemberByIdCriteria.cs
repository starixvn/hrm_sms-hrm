namespace Fanex.NexHR.Criteria.ExtensionNumber
{
    using Fanex.Data.Repository;

    public class GetExtensionNumberMemberByIdCriteria : CriteriaBase
    {
        public bool IsInternal { get; set; }

        [SpParam(SqlParamName = "EmpExtID")]
        public int EmployeeExtensionNoId { get; set; }

        public override string GetSettingKey()
        {
            return "ExtensionNO_GetEmployeeInfo";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
