namespace GO.Website.Models.Manpower
{
    using System;

    [Serializable]
    public class MemberAnnualManpowerForecastRequest
    {
        public int RequestId { get; set; }
        public string UnitName { get; set; }
        public byte StatusId { get; set; }
        public byte? CurrentProcessId { get; set; }
        public int VerifierId { get; set; }
        public int ApproverId { get; set; }
        public EmployeeInfo Creator { get; set; }
    }
}