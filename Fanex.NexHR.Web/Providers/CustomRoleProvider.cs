// --------------------------------------------------------------------------
// <copyright file="CustomRoleProvider.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Web.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Security;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interface;

    /// <summary>
    /// CustomRole Provider.
    /// </summary>
    public class CustomRoleProvider : RoleProvider
    {
        private IEmployeeService _employeeService;

        public CustomRoleProvider(IEmployeeService employeeService = null)
        {
            _employeeService = employeeService ?? new EmployeeService();
        }

        public CustomRoleProvider()
            : this(null)
        { 
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
            var roles = new List<string>();
            UserLoginModel user = GetCurrentUser(username);
            if (user != null)
            {
                roles.Add(RoleSystem.Member.EnumToString());
                if (user.RoleList != null)
                {
                    foreach (var item in user.RoleList)
                    {
                        switch (item)
                        {
                            case (int)RoleSystem.Verifier:
                                roles.Add(RoleSystem.Verifier.EnumToString());
                                break;
                            case (int)RoleSystem.Approver:
                                roles.Add(RoleSystem.Approver.EnumToString());
                                break;
                            case (int)RoleSystem.ShiftManager:
                                roles.Add(RoleSystem.ShiftManager.EnumToString());
                                break;
                            case (int)RoleSystem.ActingSupervisor:
                                roles.Add(RoleSystem.ActingSupervisor.EnumToString());
                                break;
                            case (int)RoleSystem.HR:
                                roles.Add(RoleSystem.HR.EnumToString());
                                break;
                            case (int)RoleSystem.SystemAdmin:
                                roles.Add(RoleSystem.Verifier.EnumToString());
                                roles.Add(RoleSystem.Approver.EnumToString());
                                roles.Add(RoleSystem.ShiftManager.EnumToString());
                                roles.Add(RoleSystem.ActingSupervisor.EnumToString());
                                roles.Add(RoleSystem.HR.EnumToString());
                                roles.Add(RoleSystem.SystemAdmin.EnumToString());
                                break;
                        }
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

        private UserLoginModel GetCurrentUser(string username)
        {
            var criteria = new GetUserLoginByDomainUserNameCriteria() { DomainUsername = username };
            var user = _employeeService.GetUserLoginByDomainUserName(criteria);

            return user;
        }
    }
}
