namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;

    public class TeamRepository : ITeamRepository
    {
        public Team GetById(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (var multi = db.QueryMultiple(criteria))
                {
                    var team = multi.Read<Team, Department, Team>(
                             (t, de) =>
                             {
                                 t.Department = de;
                                 return t;
                             },
                             "DepartmentId")
                             .FirstOrDefault();

                    var employees = multi.Read<TeamLeader, Employee, TeamLeader>(
                        (t, e) =>
                        {
                            t.Employee = e;
                            return t;
                        },
                        "EmployeeId").ToList();

                    team.Leaders = employees.Where(e => e.IsLeader).Select(l => l.Employee).ToList();
                    team.AssistantLeaders = employees.Where(e => !e.IsLeader).Select(l => l.Employee).ToList();

                    return team;
                }
            }
        }

        public IList<Team> GetAll(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (var multi = db.QueryMultiple(criteria))
                {
                    var teams = multi.Read<Team, Department, Team>(
                            (t, de) =>
                            {
                                t.Department = de;
                                return t;
                            },
                            "DepartmentId"
                            ).ToList();

                    var employees = multi.Read<TeamLeader, Employee, TeamLeader>(
                        (t, e) =>
                        {
                            t.Employee = e;
                            return t;
                        },
                        "EmployeeId").ToList();

                    foreach (var team in teams)
                    {
                        team.Leaders = employees.Where(e => e.Id == team.Id && e.IsLeader).Select(l => l.Employee).ToList();
                        team.AssistantLeaders = employees.Where(e => e.Id == team.Id && !e.IsLeader).Select(l => l.Employee).ToList();
                    }
                    return teams;
                }
            }
        }
    }
}