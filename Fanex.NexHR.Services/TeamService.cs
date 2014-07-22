// --------------------------------------------------------------------------
// <copyright file="TeamService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.NexHR.Criteria.Team;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services.Interface;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// Team Service.
    /// </summary>
    public class TeamService : ITeamService
    {
        private readonly ITeamRepository _teamRepository;

        public TeamService(ITeamRepository teamRespository = null)
        {
            this._teamRepository = teamRespository ?? new TeamRepository();
        }

        public Team Get(int teamId)
        {
            Team team = new Team();
            if (teamId > 0)
            {
                var allTeams = this.GetAll();
                team = allTeams.Where(t => t.Id == teamId).SingleOrDefault();
            }

            return team;
        }

        public IList<Team> GetAll()
        {
            return this._teamRepository.GetAll();
        }

        public int AddNewTeam(TeamViewModel model, out int errorCode, out string message)
        {
            return this._teamRepository.AddNewTeam(model, out errorCode, out message);
        }

        public bool UpdateTeam(TeamViewModel model, out int errorCode, out string message)
        {
            return this._teamRepository.UpdateTeam(model, out errorCode, out message);
        }

        public bool DeleteTeam(int id, DateTime modifiedDate, int modifiedby, out int errCode, out string message)
        {
            return this._teamRepository.DeleteTeam(id, modifiedDate, modifiedby, out errCode, out message);
        }

        public IList<SupervisorModel> GetTeamSupervisor(GetTeamSupervisorCriteria criteria)
        {
            IList<SupervisorModel> supervisors = null;
            if (criteria.IsValid())
            {
                supervisors = _teamRepository.GetTeamSupervisor(criteria);
            }

            return supervisors;
        }
    }
}