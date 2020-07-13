namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Group;

    public class TeamRepository : ITeamRepository
    {
        public Team GetById(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = db.QueryMultiple(criteria))
                {
                    var team = multi.Read<Team, Section, Department, Team>(
                            (t, sec, dep) =>
                            {
                                t.Section = sec;
                                t.Department = dep;
                                return t;
                            },
                             "DepartmentId, Code").FirstOrDefault();

                    if (team != null)
                    {
                        team.Managers = multi.Read<TeamManager>();
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
                    var teams = multi.Read<Team, Section , Department , Team>(
                            (t, sec, dep) =>
                            {
                                t.Section = sec;
                                t.Department = dep;
                                return t;
                            },
                             "SectionId, Code"
                            ).ToList();
                    var employees = multi.Read<TeamManager>();

                    foreach (var team in teams)
                    {
                        team.Managers = employees.Where(e => e.TeamId == team.Id);
                    }

                    return teams;
                }
            }
        }
    }
}