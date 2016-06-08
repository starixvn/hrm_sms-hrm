namespace Fanex.NexHR.Criteria.Message
{
    using System;
    using Fanex.Data.Repository;

    public class GetMessageByEmployeeCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public DateTime EffectedDate { get; set; }

        public int? IsDisabled { get; set; }

        public override string GetSettingKey()
        {
            return "Message_GetByEmployee";
        }

        public override bool IsValid()
        {
            return EmployeeId > 0;
        }
    }
}
