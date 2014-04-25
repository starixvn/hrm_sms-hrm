// --------------------------------------------------------------------------
// <copyright file="UserLoginModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.11.21">Created</change>
// </history>

namespace Fanex.NexHR.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Attributes;
    using Fanex.NexHR.Core.Enums;

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

        public IList<short> RoleList
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Roles))
                {
                    return null;
                }

                var roles = Roles.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                var result = new List<short>();

                foreach (var item in roles)
                {
                    result.Add(short.Parse(item));
                }

                return result;
            }
        }

        /// <summary>
        /// Check user is in specific roles.
        /// </summary>
        /// <param name="roles">Role of user.</param>
        /// <returns>Return Boolean.</returns>
        public bool IsInRoles(params RolesEnum[] roles)
        {
            if (RoleList == null)
            {
                return false;
            }

            var userRoles = System.Web.Security.Roles.GetRolesForUser();
            return roles.Any(role => userRoles.Contains(role.EnumToString()));
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
            return Roles == null || Roles == string.Empty || (RoleList.Count == 1 && RoleList[0] == (short)RolesEnum.Member);
        }
    }
}
