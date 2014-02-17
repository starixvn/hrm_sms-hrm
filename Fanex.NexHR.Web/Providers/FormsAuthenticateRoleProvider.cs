// --------------------------------------------------------------------------
// <copyright file="FormsAuthenticateRoleProvider.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Kan.Nguyen" date="2013.6.21">Created</change>
// </history>

namespace Fanex.NexHR.Web.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Web.Security;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interface;

    /// <summary>
    /// Forms Authenticate Role Provider.
    /// </summary>
    public class FormsAuthenticateRoleProvider : RoleProvider
    {
        private readonly IEmployeeService _employeeService;

        public FormsAuthenticateRoleProvider()
            : this(null)
        {
        }

        public FormsAuthenticateRoleProvider(IEmployeeService employeeService = null)
        {
            _employeeService = employeeService ?? new EmployeeService();
        }

        public override string ApplicationName
        {
            get
            {
                return "NexHR";
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Function to get role for users.
        /// </summary>
        /// <param name="username">User name.</param>
        /// <returns>Return array role.</returns>
        public override string[] GetRolesForUser(string username)
        {
            var user = GetCurrentUser(username);
            var roles = new List<string>();
            roles.Add(RolesEnum.Member.EnumToString());
            if (user.RoleList != null)
            {
                foreach (var item in user.RoleList)
                {
                    switch (item)
                    {
                        case (int)RolesEnum.Verifier:
                            roles.Add(RolesEnum.Verifier.EnumToString());
                            break;
                        case (int)RolesEnum.Approver:
                            roles.Add(RolesEnum.Approver.EnumToString());
                            break;
                        case (int)RolesEnum.ShiftManager:
                            roles.Add(RolesEnum.ShiftManager.EnumToString());
                            break;
                        case (int)RolesEnum.ActingSupervisor:
                            roles.Add(RolesEnum.ActingSupervisor.EnumToString());
                            break;
                        case (int)RolesEnum.HR:
                            roles.Add(RolesEnum.HR.EnumToString());
                            break;
                        case (int)RolesEnum.SystemAdmin:
                            roles.Add(RolesEnum.Verifier.EnumToString());
                            roles.Add(RolesEnum.Approver.EnumToString());
                            roles.Add(RolesEnum.ShiftManager.EnumToString());
                            roles.Add(RolesEnum.ActingSupervisor.EnumToString());
                            roles.Add(RolesEnum.HR.EnumToString());
                            roles.Add(RolesEnum.SystemAdmin.EnumToString());
                            break;
                    }
                }
            }

            return roles.ToArray();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }

        private Employee GetCurrentUser(string code)
        {
            var user = _employeeService.GetEmployeeByCode(code);
            if (user == null)
            {
                throw new Exception(code + " not found!!!");
            }

            return user;
        }
    }
}
