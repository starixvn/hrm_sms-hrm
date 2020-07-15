namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.EmployeeInformation;

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

                    var roles = multi.Read<EmployeeRole, SystemRoleModel, EmployeeRole>(
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

                    var roles = multi.Read<EmployeeRole, SystemRoleModel, EmployeeRole>(
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

                    var relationships = multi.Read<EmployeeRelationship>();

                    profiles = GroupRelationship(profiles, relationships);

                    return profiles.ToList();
                }
            }
        }

        private static IEnumerable<EmployeeFullProfile> GroupRelationship(
            IEnumerable<EmployeeFullProfile> profiles,
            IEnumerable<EmployeeRelationship> relationships)
        {
            foreach (var profile in profiles)
            {
                profile.Relationships = relationships.Where(r => r.EmployeeId == profile.EmpId);
            }

            return profiles;
        }
    }
}