namespace GO.Website.Criteria.Manpower
{
    using GO.Core.Models;

    public class UpdateExcludedEmployeeCommand : BaseNonQueryCommand
    {
        public short SiteId { get; set; }
        public int EmpId { get; set; }
        public bool IsExcluded { get; set; }
        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "Manpower_ExcludedEmployee_Update";

        public override bool IsValid() => SiteId > 0 && EmpId > 0 && ModifiedBy > 0;
    }
}