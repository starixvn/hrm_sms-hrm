// --------------------------------------------------------------------------
// <copyright file="UserLoginModel.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.11.21">Created</change>
// </history>

namespace Fanex.NexHR.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Security;
    using Fanex.NexHR.Core.Enums;
    using Nexus.RoleManagement.Services;

    /// <summary>
    /// UserLogin Model.
    /// </summary>
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

        public IEnumerable<FunctionModel> NE_Functions { get; set; }

        public IEnumerable<MenuModel> NE_Menus { get; set; }

        public IList<short> RoleList
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Roles))
                {
                    return null;
                }

                var roles = Roles.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                return roles.Select(role => short.Parse(role)).ToList();
            }
        }

        public string DisplayName
        {
            get
            {
                return (string.IsNullOrEmpty(NickName) || string.IsNullOrEmpty(FullName)) ? string.Empty : NickName + " (" + FullName + ")";
            }
        }

        /// <summary>
        /// Check user is in specific roles.
        /// </summary>
        /// <param name="roles">Role of user.</param>
        /// <returns>Return Boolean.</returns>
        public bool IsInRoles(params RoleSystem[] roles)
        {
            if (RoleList == null)
            {
                return false;
            }

            var userRoles = System.Web.Security.Roles.GetRolesForUser();
            return roles.Any(role => userRoles.Contains(role.ToString()));
        }

        /// <summary>
        /// Check user is in specific roles.
        /// </summary>
        /// <param name="roles">Role of user.</param>
        /// <returns>Return Boolean.</returns>
        public bool IsInRoles(IList<string> roles)
        {
            if (roles == null || roles.Count == 0)
            {
                return false;
            }

            var userRoles = System.Web.Security.Roles.GetRolesForUser();
            return roles.Any(role => userRoles.Contains(role));
        }

        public bool IsMember()
        {
            return string.IsNullOrWhiteSpace(Roles) || (RoleList.Count == 1 && RoleList[0] == (short)RoleSystem.Member);
        }

        public bool HasPermission(string functionKey, Policy policy)
        {
            var function = NE_Functions.Where(i => i.FunctionKey.Equals(functionKey));
            if (function.Any())
            {
                switch (policy)
                {
                    case Policy.Create:
                        return function.First().IsCreate;
                    case Policy.Edit:
                        return function.First().IsEdit;
                    case Policy.Delete:
                        return function.First().IsDelete;
                    default:
                        return function.First().IsRead;
                }
            }

            return false;
        }
    }
}
