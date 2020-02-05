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
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = db.QueryMultiple(criteria))
                {
                    var team = multi.Read<Team, Department, Team>(
                             (t, de) =>
                             {
                                 t.Department = de;
                                 return t;
                             },
                             "DepartmentId").FirstOrDefault();
                    var employees = multi.Read<TeamLeader>();

                    if (team != null)
                    {
                        team.Leaders = employees.Where(e => e.IsLeader).ToList();
                        team.AssistantLeaders = employees.Where(e => !e.IsLeader).ToList();
                    }

                    return team;
                }
            }
        }

        public IList<Team> GetAll(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
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
                    var employees = multi.Read<TeamLeader>();

                    foreach (var team in teams)
                    {
                        team.Leaders = employees.Where(e => e.GroupId == team.Id && e.IsLeader).ToList();
                        team.AssistantLeaders = employees.Where(e => e.GroupId == team.Id && !e.IsLeader).ToList();
                    }

                    return teams;
                }
            }
        }
    }
}