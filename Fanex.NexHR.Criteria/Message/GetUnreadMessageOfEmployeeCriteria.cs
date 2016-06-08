namespace Fanex.NexHR.Criteria.Message
{
    using Fanex.Data.Repository;

    public class GetUnreadMessageOfEmployeeCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public override string GetSettingKey()
        {
            return "Message_GetUnreadMessageByEmployee";
        }

        public override bool IsValid()
        {
            return EmployeeId > 0;
        }
    }
}
