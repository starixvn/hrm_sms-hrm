namespace Fanex.NexHR.Models
{
    using Nexus.RoleManagement.Services;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Security;

    [Serializable]
    public class UserLoginModel
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string FullName { get; set; }

        public string NickName { get; set; }

        public int? JobTitleId { get; set; }

        public string CountryCode { get; set; }

        public string Roles { get; set; }

        public bool IsShowLeftNavigation { get; set; }

        public bool IsTopManager { get; set; }

        public bool CanOT { get; set; }

        public bool PasswordReset { get; set; }

        public IEnumerable<SystemRoleModel> NE_Roles { get; set; }

        public IEnumerable<FunctionViewModel> NE_Functions { get; set; }

        public IEnumerable<MenuModel> NE_Menus { get; set; }

        public string DisplayName
        {
            get
            {
                return (string.IsNullOrEmpty(NickName) || string.IsNullOrEmpty(FullName)) ? string.Empty : NickName + " (" + FullName + ")";
            }
        }

        public bool HasPermission(string functionKey, Policy policy)
        {
            var function = NE_Functions.Where(i => i.FunctionKey.Equals(functionKey));
            if (function.Any())
            {
                switch (policy)
                {
                    case Policy.Host:
                        return function.First().IsHost;
                    default:
                        return function.First().IsRead;
                }
            }

            return false;
        }
    }
}
