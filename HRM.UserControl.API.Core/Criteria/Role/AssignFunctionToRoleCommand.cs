namespace HRM.UserControl.API.Core.Criteria.Role
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fanex.Data;
    using HRM.UserControl.Core.Models;
    using HRM.UserControl.Models;
    using HRM.UserControl.Core.Extensions.XML;

    public class AssignFunctionToRoleCommand : BaseNonQueryCommand
    {
        public short RoleId { get; set; }

        public string Value
        {
            get
            {
                var value = new StringBuilder("<Root>");

                if (Permissions != null && Permissions.Any())
                {
                    Permissions.ToXML();
                    const string AssignPermissionTpl = "<Row FunctionId='{0}' IsRead='{1}' IsHost='{2}' />";
                    foreach (var permission in Permissions)
                    {
                        value.AppendFormat(AssignPermissionTpl, permission.FunctionId, permission.IsRead, permission.IsHost);
                    }
                }

                return value.Append("</Root>").ToString();
            }
        }

        public short SiteId { get; set; }
        public int ModifiedBy { get; set; }

        [SpParam(Ignored = true)]
        public IList<FunctionPermission> Permissions { get; set; }

        public override string GetSettingKey() => "API_Role_AssignFunction";

        public override bool IsValid() => RoleId > 0 && SiteId > 0 && ModifiedBy > 0;
    }
}