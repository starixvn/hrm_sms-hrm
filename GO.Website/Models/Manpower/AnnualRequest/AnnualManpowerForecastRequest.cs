namespace GO.Website.Models.Manpower
{
    using System;
    using System.Collections.Generic;
    using GO.Website.Models.Manpower.Period;

    [Serializable]
    public class AnnualManpowerForecastRequest
    {
        public int RequestId { get; set; }
        public string UnitName { get; set; }
        public byte StatusId { get; set; }
        public byte? CurrentProcessId { get; set; }
        public bool IsEdit { get; set; }
        public ManpowerPeriod Period { get; set; }

        public IList<AnnualManpowerForecastRequestDetail> Details { get; set; }

        public EmployeeInfo Creator { get; set; }
        public EmployeeInfo Verifier { get; set; }
        public EmployeeInfo Approver { get; set; }
    }
}