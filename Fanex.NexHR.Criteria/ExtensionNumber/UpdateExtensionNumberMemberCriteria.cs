namespace Fanex.NexHR.Criteria.ExtensionNumber
{
    using Fanex.Data.Repository;

    public class UpdateExtensionNumberMemberCriteria : CriteriaBase
    {
        public int ExtID { get; set; }

        public string FullName { get; set; }

        public string NickName { get; set; }

        public string ExtensionNo { get; set; }

        public bool IsInternal { get; set; }

        public int EmpID { get; set; }

        public int GroupID { get; set; }

        public short OrderNo { get; set; }

        public override string GetSettingKey()
        {
            return "ExtensionNO_Update_EmployeeInfo";
        }

        public override bool IsValid()
        {
            return ExtID > 0;
        }
    }
}
