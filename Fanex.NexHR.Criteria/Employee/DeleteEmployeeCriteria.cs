namespace Fanex.NexHR.Criteria.Employee
{
    using System;
    using Fanex.Data.Repository;

    public class DeleteEmployeeCriteria : CriteriaBase
    {
        public int EmpId { get; set; }

        public DateTime ResignedDate { get; set; }

        public override string GetSettingKey()
        {
            return "Employee_Delete";
        }

        public override bool IsValid()
        {
            return EmpId > 0;
        }
    }
}
