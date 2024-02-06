namespace Fanex.NexHR.Criteria.Employee
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetAllEmployeeCriteria : CriteriaBase
    {
        public short CompanyId { get; set; }
        public string Query { get; set; }

        public string TeamIds { get; set; }

        public short? PageNumber { get; set; }

        public short? PageSize { get; set; }

        public int TotalRow { get; set; }

        public int? TitleId { get; set; }

        [SpParam(SqlParamName = "Enable")]
        public bool? EnableUser { get; set; }

        [SpParam(SqlParamName = "IsDelete")]
        public bool? IsDeleteUser { get; set; }

        [SpParam(SqlParamName = "IsLocal")]
        public bool? IsLocalUser { get; set; }

        public override string GetSettingKey() => "Employee_GetAll";

        public override bool IsValid() => true;
    }
}