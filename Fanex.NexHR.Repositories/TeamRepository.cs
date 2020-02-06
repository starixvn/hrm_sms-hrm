namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Section;

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

                    if (team != null)
                    {
                        team.Managers = multi.Read<SectionManager>();
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
                    var employees = multi.Read<SectionManager>();

                    foreach (var team in teams)
                    {
                        team.Managers = employees.Where(e => e.GroupId == team.Id);
                    }

                    return teams;
                }
            }
        }
    }
}