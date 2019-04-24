namespace Fanex.NexHR.Criteria.Function
{
    using Fanex.Data.Repository;

    public class AssignFunctionToRoleCriteria : CriteriaBase
    {
        public int ActorId { get; set; }
        public int RoleId { get; set; }
        public string PermissionXml { get; set; }

        public override bool IsValid() => ActorId > 0 && RoleId > 0;

        public override string GetSettingKey() => "Function_AssignFunctionToRole";
    }
}