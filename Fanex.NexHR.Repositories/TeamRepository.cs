namespace Fanex.NexHR.Repositories
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories.Interfaces;
    using System.Collections.Generic;
    using System.Linq;

    public class TeamRepository : ITeamRepository
    {
        public Team GetById(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var team = db.QueryEntity<Team, Department, Employee, Employee, Team>(
                        (t, de, ld, asld) =>
                        {
                            t.Department = de;
                            t.Leader = ld;
                            t.AssistantLeader = asld;
                            return t;
                        },
                        splitOn: "DepartmentId, leaderID, AssistantLeaderId",
                        parameters: criteria.GetSpParams());

                return team;
            }
        }

        public IList<Team> GetAll(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var teams = db.Query<Team, Department, Employee, Employee, Team>(
                        (t, de, ld, asld) =>
                        {
                            t.Department = de;
                            t.Leader = ld;
                            t.AssistantLeader = asld;
                            return t;
                        },
                        splitOn: "DepartmentId, leaderID, AssistantLeaderId",
                        parameterValues: criteria.GetSpParams()).ToList();

                return teams;
            }
        }
    }
}
