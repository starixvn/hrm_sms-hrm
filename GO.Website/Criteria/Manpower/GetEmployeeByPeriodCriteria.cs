namespace GO.Website.Criteria.Manpower
{
    using Fanex.Data.Repository;

    public class GetEmployeeByPeriodCriteria : CriteriaBase
    {
        public short PeriodId { get; set; }

        public override string GetSettingKey() => "Manpower_Employee_GetByPeriodId";

        public override bool IsValid() => PeriodId > 0;
    }
}