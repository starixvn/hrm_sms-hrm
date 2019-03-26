namespace Fanex.NexHR.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.NexHR.Core.Enums;

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

        public bool IsShowLeftNavigation { get; set; }

        public bool IsTopManager { get; set; }

        public bool CanOT { get; set; }

        public bool PasswordReset { get; set; }

        public IEnumerable<SystemRoleModel> Roles { get; set; }

        public IEnumerable<FunctionModel> Functions { get; set; }

        public IEnumerable<MenuModel> Menus { get; set; }

        public string DisplayName
        {
            get
            {
                return (string.IsNullOrEmpty(NickName) || string.IsNullOrEmpty(FullName)) ? string.Empty : NickName + " (" + FullName + ")";
            }
        }

        public bool HasPermission(string functionKey, Policy policy)
        {
            var function = Functions.Where(i => i.FunctionKey.Equals(functionKey));
            if (function.Any())
            {
                if (policy == Policy.Host)
                {
                    return function.First().IsHost;
                }

                return function.First().IsRead;
            }

            return false;
        }
    }
}