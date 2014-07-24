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
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// Team Repository.
    /// </summary>
    public class TeamRepository : ITeamRepository
    {
        /// <summary>
        /// Get Team by id.
        /// </summary>
        /// <param name="teamId">Team's id.</param>
        /// <returns>Team entity.</returns>
        public Team GetById(int teamId)
        {
            using (var db = new ObjectDb("Group_Select"))
            {
                var team = db.QueryEntity<Team, Employee, Employee, Team>(
                        (t, ld, asld) =>
                        {
                            t.Leader = ld;
                            t.AssistantLeader = asld;
                            return t;
                        },
                        splitOn: "leaderID, AssistantLeaderId",
                        parameters: new { groupId = teamId });

                return team;
            }
        }

        /// <summary>
        /// Get all Team.
        /// </summary>
        /// <returns>List of Team.</returns>
        public IList<Team> GetAll()
        {
            using (var db = new ObjectDb("Group_Select"))
            {
                var teams = db.Query<Team, Employee, Employee, Team>(
                        (t, ld, asld) =>
                        {
                            t.Leader = ld;
                            t.AssistantLeader = asld;
                            return t;
                        },
                        splitOn: "leaderID, AssistantLeaderId").ToList();

                return teams;
            }
        }

        /// <summary>
        /// Method update Team.
        /// </summary>
        /// <param name="model">Team ViewModel.</param>
        /// <param name="errorCode">Error code.</param>
        /// <param name="message">Error message.</param>
        /// <returns>Boolean to check status.</returns>
        public bool UpdateTeam(TeamViewModel model, out int errorCode, out string message)
        {
            using (var db = new ObjectDb("EmployeeGroup_Update"))
            {
                db.AddInParameter("@GroupId", DbType.Int32, model.Id);
                db.AddInParameter("@Code", DbType.String, model.TeamCode);
                db.AddInParameter("@Name", DbType.String, model.TeamName);
                db.AddInParameter("@MailGroup", DbType.String, model.MailGroup);
                db.AddInParameter("@LeaderId", DbType.Int32, model.LeaderId);
                db.AddInParameter("@AssistantLeaderId", DbType.Int32, model.AssistantLeaderId);
                db.AddInParameter("@ModifiedDate", DbType.DateTime, model.ModifiedDate);
                db.AddInParameter("@ModifiedBy", DbType.Int32, model.ModifiedBy);
                db.AddOutParameter("@ErrCode", DbType.Int32, 8);
                db.AddOutParameter("@ErrMsg", DbType.String, 200);

                db.ExecuteNonQuery();
                errorCode = (int)db.GetParameterValue("ErrCode");
                message = db.GetParameterValue("ErrMsg") as string;

                return errorCode == 0;
            }
        }

        /// <summary>
        /// Method Add new Team.
        /// </summary>
        /// <param name="model">Team ViewModel.</param>
        /// <param name="errorCode">Error code.</param>
        /// <param name="message">Error Message.</param>
        /// <returns>Value of error code.</returns>
        public int AddNewTeam(TeamViewModel model, out int errorCode, out string message)
        {
            using (var db = new ObjectDb("EmployeeGroup_Insert"))
            {
                db.AddInParameter("@Code", DbType.String, model.TeamCode);
                db.AddInParameter("@Name", DbType.String, model.TeamName);
                db.AddInParameter("@MailGroup", DbType.String, model.MailGroup);
                db.AddInParameter("@LeaderId", DbType.Int32, model.LeaderId);
                db.AddInParameter("@AssistantLeaderId", DbType.Int32, model.AssistantLeaderId);
                db.AddInParameter("@CreatedDate", DbType.DateTime, model.CreatedDate);
                db.AddInParameter("@CreatedBy", DbType.Int32, model.CreatedBy);
                db.AddOutParameter("@ErrCode", DbType.Int32, 8);
                db.AddOutParameter("@ErrMsg", DbType.String, 200);
                db.ExecuteScalar();
                errorCode = (int)db.GetParameterValue("ErrCode"); 
                message = db.GetParameterValue("ErrMsg") as string;
                return errorCode;
            }
        }

        /// <summary>
        /// Method Delete Team.
        /// </summary>
        /// <param name="id">Id of Team.</param>
        /// <param name="modifiedDate">Modified Date.</param>
        /// <param name="modifiedBy">Modified By.</param>
        /// <param name="errorCode">Error Code.</param>
        /// <param name="message">Delete message.</param>
        /// <returns>Boolean to check status.</returns>
        public bool DeleteTeam(int id, DateTime modifiedDate, int modifiedBy, out int errorCode, out string message)
        {
            using (var db = new ObjectDb("EmployeeGroup_Delete"))
            {
                db.AddInParameter("@GroupID", DbType.Int32, id);
                db.AddInParameter("@ModifiedDate", DbType.DateTime, modifiedDate);
                db.AddInParameter("@ModifiedBy", DbType.Int32, modifiedBy);
                db.AddOutParameter("@ErrCode", DbType.Int32, 8);
                db.AddOutParameter("@ErrMsg", DbType.String, 200);

                var result = db.ExecuteScalar() == null ? false : true;

                message = db.GetParameterValue("ErrMsg") as string;
                errorCode = (int)db.GetParameterValue("ErrCode");

                return result;
            }
        }

        public List<SupervisorModel> GetTeamSupervisor(ICriteria criteria)
        {
            List<SupervisorModel> supervisors = null;
            using (ObjectDb db = new ObjectDb(criteria.GetSettingKey()))
            {
                supervisors = db.Query<SupervisorModel>(criteria.GetSpParams()).ToList();
            }

            return supervisors;
        }

        public List<TeamSupervisorModel> GetSupervisorByTeamId(ICriteria criteria)
        {
            List<TeamSupervisorModel> supervisor = null;
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
