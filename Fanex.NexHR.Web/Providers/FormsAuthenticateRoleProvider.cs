namespace Fanex.NexHR.Web.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Security;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;

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

        public override string[] GetRolesForUser(string username)
        {
            var roles = new List<string>();
            Employee user = GetCurrentUser(username);
            if (user != null)
            {
                roles.Add(RoleSystem.Member.ToString());
                if (user.RoleList != null)
                {
                    foreach (var item in user.RoleList)
                    {
                        switch (item)
                        {
                            case (int)RoleSystem.Verifier:
                                roles.Add(RoleSystem.Verifier.ToString());
                                break;

                            case (int)RoleSystem.Approver:
                                roles.Add(RoleSystem.Approver.ToString());
                                break;

                            case (int)RoleSystem.ShiftManager:
                                roles.Add(RoleSystem.ShiftManager.ToString());
                                break;

                            case (int)RoleSystem.ActingSupervisor:
                                roles.Add(RoleSystem.ActingSupervisor.ToString());
                                break;

                            case (int)RoleSystem.HR:
                                roles.Add(RoleSystem.HR.ToString());
                                break;

                            case (int)RoleSystem.SystemAdmin:
                                roles.Add(RoleSystem.Verifier.ToString());
                                roles.Add(RoleSystem.Approver.ToString());
                                roles.Add(RoleSystem.ShiftManager.ToString());
                                roles.Add(RoleSystem.ActingSupervisor.ToString());
                                roles.Add(RoleSystem.HR.ToString());
                                roles.Add(RoleSystem.SystemAdmin.ToString());
                                break;

                            case (int)RoleSystem.EditDiscipline:
                                roles.Add(RoleSystem.EditDiscipline.ToString());
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

        private Employee GetCurrentUser(string code)
        {
            var criteria = new GetEmployeeByCodeCriteria() { Code = code };
            Employee user = _employeeService.GetEmployeeByCode(criteria);
            return user;
        }
    }
}