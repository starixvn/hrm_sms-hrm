namespace Fanex.NexHR.Criteria.Employee
{
    using Fanex.Data.Repository;

    public class SelfUpdateContactCriteria : CriteriaBase
    {
        public int EmpId { get; set; }

        public string ChineseName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Skype { get; set; }

        public string Yahoo { get; set; }

        public string Address { get; set; }

        public string CurrentAddress { get; set; }

        public override string GetSettingKey()
        {
            return "Employee_SelfUpdateInfo";
        }

        public override bool IsValid()
        {
            return EmpId > 0;
        }
    }
}