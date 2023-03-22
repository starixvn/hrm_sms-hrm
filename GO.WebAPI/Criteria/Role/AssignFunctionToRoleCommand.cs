namespace GO.WebAPI.Criteria.Role
{
    using System.Collections.Generic;
    using Fanex.Data;
    using GO.Core.Extensions.XML;
    using GO.Core.Models;
    using GO.Models;

    public class AssignFunctionToRoleCommand : BaseNonQueryCommand
    {
        public short RoleId { get; set; }

        public string Value => Permissions.ToXML().ToString();

        public short SiteId { get; set; }
        public int ModifiedBy { get; set; }

        [SpParam(Ignored = true)]
        public IList<FunctionPermission> Permissions { get; set; }

        public override string GetSettingKey() => "API_Role_AssignFunction";

        public override bool IsValid() => RoleId > 0 && SiteId > 0 && ModifiedBy > 0;
    }
}