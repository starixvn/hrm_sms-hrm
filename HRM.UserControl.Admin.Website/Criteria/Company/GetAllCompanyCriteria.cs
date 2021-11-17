namespace HRM.UserControl.Admin.Website.Criteria.Company
{
    using Fanex.Data.Repository;

    public class GetAllCompanyCriteria : CriteriaBase
    {
        public bool? IsActive { get; set; }

        public override string GetSettingKey() => "Company_GetAll";

        public override bool IsValid() => true;
    }
}