namespace Fanex.NexHR.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Core.Attributes;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.EmployeeInformation;

    public class EmployeeRepository : IEmployeeRepository
    {
        public Employee GetEmployeeById(ICriteria criteria)
        {
            using (IObjectDb db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (var multi = db.QueryMultiple(criteria))
                {
                    Employee employee;
                    employee = multi.Read<Employee, Team, Employee>(
                        (e, t) =>
                        {
                            e.Team = t;
                            return e;
                        },
                        splitOn: "TeamId").First();

                    return employee;
                }
            }
        }

        public Employee GetEmployeeByCode(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                Employee entity = db.QueryEntity<Employee, dynamic, Employee>(
                        (e, t) =>
                        {
                            e.Team = new Team { Id = t.TeamID, Name = t.Team };
                            return e;
                        },
                        splitOn: "TeamId",
                        parameterSet: criteria);

                return entity;
            }
        }

        public IList<Employee> GetEmployees(ICriteria criteria)
        {
            using (IObjectDb db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (var multi = db.QueryMultiple(criteria))
                {
                    IEnumerable<Employee> employees = multi.Read<Employee, dynamic, Employee>(
                        (e, t) =>
                        {
                            e.Team = new Team { Id = t.TeamID, Code = t.Team, Name = t.TeamName };
                            return e;
                        },
                        splitOn: "TeamID");

                    IEnumerable<EmployeeRole> roles = multi.Read<EmployeeRole, SystemRoleModel, EmployeeRole>(
                        (er, r) =>
                        {
                            er.Role = r;

                            return er;
                        },
                        "RoleId");

                    employees = GroupByRoles(employees, roles);

                    PropertyInfo totalitem = criteria.GetType().GetProperties().FirstOrDefault(
                        prop => Attribute.IsDefined(prop, typeof(OutputAttribute)) && prop.Name.ToLowerInvariant().Equals("totalrow"));

                    if (totalitem != null)
                    {
                        var hashParam = db.GetOutputParams();
                        totalitem.SetValue(criteria, (int)hashParam["totalrow"], null);
                    }

                    return employees.ToList();
                }
            }
        }

        public IList<Employee> GetEmployeesByRole(ICriteria criteria)
        {
            using (IObjectDb db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (var multi = db.QueryMultiple(criteria))
                {
                    IEnumerable<Employee> employees = multi.Read<Employee, dynamic, Employee>(
                        (e, t) =>
                        {
                            e.Team = new Team { Id = t.TeamID, Name = t.Team };
                            return e;
                        },
                        splitOn: "TeamID");

                    IEnumerable<EmployeeRole> roles = multi.Read<EmployeeRole, SystemRoleModel, EmployeeRole>(
                            (er, r) =>
                            {
                                er.Role = r;

                                return er;
                            },
                            "RoleId");

                    employees = GroupByRoles(employees, roles);

                    return employees.ToList();
                }
            }
        }

        private static IEnumerable<Employee> GroupByRoles(IEnumerable<Employee> employees, IEnumerable<EmployeeRole> roles)
        {
            foreach (var employee in employees)
            {
                employee.Roles = roles.Where(i => i.EmployeeId == employee.Id).Select(i => i.Role).ToList();
            }

            return employees;
        }

        public IList<EmployeeFullProfile> GetEmployeeFullInformation(ICriteria criteria)
        {
            using (IObjectDb db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (var multi = db.QueryMultiple(criteria))
                {
                    IEnumerable<EmployeeFullProfile> profiles = multi.Read<EmployeeFullProfile, EmployeeEmergency, EmployeeFullProfile>(
                        (f, em) =>
                        {
                            f.Emergency = em;
                            return f;
                        },
                        splitOn: "EmergencyId");

                    IEnumerable<EmployeeRelationship> relationships = multi.Read<EmployeeRelationship>();

                    profiles = GroupRelationship(profiles, relationships);

                    return profiles.ToList();
                }
            }
        }

        private static IEnumerable<EmployeeFullProfile> GroupRelationship(IEnumerable<EmployeeFullProfile> profiles, IEnumerable<EmployeeRelationship> relationships)
        {
            foreach (var profile in profiles)
            {
                profile.Relationships = relationships.Where(r => r.EmployeeId == profile.EmpId);
            }

            return profiles;
        }
    }
}