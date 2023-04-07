namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.EmployeeInformation;
    using Fanex.NexHR.Models.Supervisor;
    using HRM.UserControl.Client.Core.Models.Role;

    public class EmployeeRepository : IEmployeeRepository
    {
        public Employee GetEmployeeById(ICriteria criteria)
        {
            using (IObjectDb db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.QueryEntity<Employee, Team, Employee>(
                        (e, t) =>
                        {
                            e.Team = t;
                            return e;
                        },
                        splitOn: "TeamId",
                        parameterSet: criteria);
            }
        }

        public Employee GetEmployeeByCode(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.QueryEntity<Employee, dynamic, Employee>(
                        (e, t) =>
                        {
                            e.Team = new Team { Id = t.TeamID, Name = t.Team };
                            return e;
                        },
                        splitOn: "TeamId",
                        parameterSet: criteria);
            }
        }

        public IList<Employee> GetEmployees(ICriteria criteria)
        {
            using (IObjectDb db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (var multi = db.QueryMultiple(criteria))
                {
                    var employees = multi.Read<Employee, dynamic, Employee>(
                        (e, t) =>
                        {
                            e.Team = new Team {
                                Id = t.TeamID,
                                Code = t.Team,
                                Name = t.TeamName,
                                Section = new Section
                                {
                                    Id = t.SectionId,
                                    Code = t.SectionCode,
                                    Department = new Department
                                    {
                                        Id = t.DepartmentId,
                                        Code = t.DepartmentCode
                                    }
                                }
                            };
                            return e;
                        },
                        splitOn: "TeamID");

                    var roles = multi.Read<EmployeeRole, Role, EmployeeRole>(
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

        public IList<Employee> GetEmployeesByRole(ICriteria criteria)
        {
            using (IObjectDb db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (var multi = db.QueryMultiple(criteria))
                {
                    var employees = multi.Read<Employee, dynamic, Employee>(
                        (e, t) =>
                        {
                            e.Team = new Team { Id = t.TeamID, Name = t.Team };
                            return e;
                        },
                        splitOn: "TeamID");

                    var roles = multi.Read<EmployeeRole, Role, EmployeeRole>(
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
                    var profiles = multi.Read<EmployeeFullProfile>();

                    var supervisor = multi.Read<SupervisorNameModel>();

                    var roles = multi.Read<EmployeeRole, Role, EmployeeRole>(
                            (er, r) =>
                            {
                                er.Role = r;
                                return er;
                            },
                            "RoleId");

                    profiles = GroupBySupAndRoles(profiles, supervisor, roles);

                    return profiles.ToList();
                }
            }
        }

        private static IEnumerable<EmployeeFullProfile> GroupBySupAndRoles(
            IEnumerable<EmployeeFullProfile> profiles,
            IEnumerable<SupervisorNameModel> supervisor,
            IEnumerable<EmployeeRole> roles)
        {
            foreach (var profile in profiles)
            {
                profile.Supervisor = supervisor.Where(i => i.EmpId == profile.EmpId).ToList();
                profile.Roles = roles.Where(i => i.EmployeeId == profile.EmpId).Select(i => i.Role).ToList();
            }

            return profiles;
        }
    }
}