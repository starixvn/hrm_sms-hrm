namespace Nexus.RoleManagement.Services
{
    using Fanex.Data.Repository;

    public class ResultCriteria : CriteriaBase
    {
        public int ErrorCode { get; set; }

        public string ErrorMessage { get; set; }
    }
}
