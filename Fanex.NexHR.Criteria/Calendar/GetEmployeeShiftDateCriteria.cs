namespace Fanex.NexHR.Criteria.Shift
{
    using System;
    using Fanex.Data.Repository;

    public class GetEmployeeShiftDateCriteria : CriteriaBase
    {
        public int SupervisorId { get; set; }

        public int? ShiftId { get; set; }

        public int? EmployeeId { get; set; }

        public DateTime WorkDate { get; set; }

        public string TeamIds { get; set; }

        public bool IsShowWholeCompany { get; set; }

        public byte? IsLocal { get; set; }

        public override string GetSettingKey()
        {
            return "Employee_Shift_GetByDate";
        }

        public override bool IsValid()
        {
            return SupervisorId > 0;
        }
    }
}
