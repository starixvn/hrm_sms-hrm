namespace Fanex.NexHR.Repositories
{
    using System;
    using System.Reflection;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Core.Attributes;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories.Interfaces;
    using Nexus.RoleManagement.Services;
    using System.Collections.Generic;
    using System.Linq;

    public class EmployeeRepository : IEmployeeRepository
    {
        public Employee GetEmployeeById(ICriteria criteria)
        {
            using (IObjectDb db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (var multi = db.QueryMultiple(criteria))
                {
                    var employee = new Employee();
                    employee = multi.Read<Employee, dynamic, Employee>(
                        (e, t) =>
                        {
                            e.Team = new Team
                            {
                                Id = t.TeamID,
                                Name = t.Team,
                                Department = new Department
                                {
                                    Id = t.DepartmentId == null ? -1 : t.DepartmentId
                                }
                            };
                            return e;
                        },
                        splitOn: "TeamID").First();

                    employee.NX_Roles = multi.Read<SystemRoleModel>().ToList();

                    return employee;
                }
            }
        }

        public Employee GetEmployeeByCode(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                Employee entity = db.QueryEntity<Employee, dynamic, Employee>(
                        (e, t) =>
                        {
                            e.Team = new Team { Id = t.TeamID, Name = t.Team };
                            return e;
                        },
                        splitOn: "TeamID",
                        parameters: criteria.GetSpParams());

                return entity;
            }
        }

        public IList<Employee> GetEmployees(ICriteria criteria)
        {
            using (IObjectDb db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (var multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    var employees = multi.Read<Employee, dynamic, Employee>(
                        (e, t) =>
                        {
                            e.Team = new Team { Id = t.TeamID, Code = t.Team, Name = t.TeamName };
                            return e;
                        },
                        splitOn: "TeamID");

                    var roles = multi.Read<EmployeeRole, SystemRoleModel, EmployeeRole>(
                        (er, r) =>
                        {
                            er.Role = r;

                            return er;
                        },
                        "RoleId");
                    foreach (var employee in employees)
                    {
                        employee.NX_Roles = roles.Where(i => i.EmployeeId == employee.Id).Select(i => i.Role).ToList();
                    }

                     PropertyInfo totalitem = criteria.GetType().GetProperties().Where(
                        prop => Attribute.IsDefined(prop, typeof(OutputAttribute)) && prop.Name.ToLower().Equals("totalrow")).FirstOrDefault();

                if (totalitem != null)
                {
                    var hashParam = db.GetOuputParams();
                    totalitem.SetValue(criteria, (int)hashParam["totalrow"], null);
                }

                    return employees.ToList();
                }
            }
        }

        public IList<Employee> GetEmployeesByRole(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var entity = db.Query<Employee, dynamic, Employee>(
                        (e, t) =>
                        {
                            e.Team = new Team { Id = t.TeamID, Name = t.Team };
                            return e;
                        },
                        splitOn: "TeamID",
                        parameterValues: criteria.GetSpParams()).ToList();

                return entity;
            }
        }
    }
}
