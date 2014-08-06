// --------------------------------------------------------------------------
// <copyright file="TeamRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Supervisor;
    using Fanex.NexHR.Repositories.Interfaces;

    /// <summary>
    /// Team Repository.
    /// </summary>
    public class TeamRepository : ITeamRepository
    {
        /// <summary>
        /// Get Team by id.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Team entity.</returns>
        public Team GetById(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var team = db.QueryEntity<Team, Employee, Employee, Team>(
                        (t, ld, asld) =>
                        {
                            t.Leader = ld;
                            t.AssistantLeader = asld;
                            return t;
                        },
                        splitOn: "leaderID, AssistantLeaderId",
                        parameters: criteria.GetSpParams());

                return team;
            }
        }

        /// <summary>
        /// Get all Team.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>List of Team.</returns>
        public IList<Team> GetAll(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var teams = db.Query<Team, Employee, Employee, Team>(
                        (t, ld, asld) =>
                        {
                            t.Leader = ld;
                            t.AssistantLeader = asld;
                            return t;
                        },
                        splitOn: "leaderID, AssistantLeaderId",
                        parameterValues: criteria.GetSpParams()).ToList();

                return teams;
            }
        }

        public IList<SupervisorModel> GetTeamSupervisor(ICriteria criteria)
        {
            IList<SupervisorModel> supervisors = null;
            using (ObjectDb db = new ObjectDb(criteria.GetSettingKey()))
            {
                supervisors = db.Query<SupervisorModel>(criteria.GetSpParams()).ToList();
            }

            return supervisors;
        }

        public IList<TeamSupervisorModel> GetSupervisorByTeamId(ICriteria criteria)
        {
            IList<TeamSupervisorModel> supervisor = null;
            using (ObjectDb db = new ObjectDb(criteria.GetSettingKey()))
            {
                supervisor = db.Query<TeamSupervisorModel>(criteria.GetSpParams()).ToList();
            }

            return supervisor;
        }

        public bool UpdateSupervisor(ICriteria criteria)
        {
            bool updated = false;
            using (ObjectDb db = new ObjectDb(criteria.GetSettingKey()))
            {
                updated = db.ExecuteNonQuery(criteria.GetSpParams()) != 0;
            }

            return updated;
        }

        public bool InsertTeamShiftManager(ICriteria criteria)
        {
            bool status = false;
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                status = (int)db.ExecuteScalar(criteria.GetSpParams()) == 0;
            }

            return status;
        }
    }
}
