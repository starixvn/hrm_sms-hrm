namespace Fanex.NexHR.Criteria.Supervisor
{
    using Fanex.Data.Repository;

    public class InsertShiftManagerCriteria : CriteriaBase
    {
        public int EmpId { get; set; }

        public string EmpIdShift { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "EmployeeSupervisor_InsertEmpMngShiftList";
        }

        public override bool IsValid()
        {
            return EmpId > 0;
        }
    }
}
