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
    using Fanex.Common.Helper;
    using Fanex.NexHR.Criteria.GroupSupervisor;
    using Fanex.NexHR.Criteria.Team;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services.Interface;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// Team Service.
    /// </summary>
    public class TeamService : BaseService, ITeamService
    {
        private readonly ITeamRepository _teamRepository;

        public TeamService()
            : this(null)
        {
        }

        public TeamService(ITeamRepository teamRepository)
        {
            this._teamRepository = teamRepository ?? new TeamRepository();
        }

        /// <summary>
        /// Get group by group Id.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The Group object.</returns>
        public Team GetById(GetGroupByIdCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            Team team = new Team();

            if (criteria.IsValid())
            {
                team = _teamRepository.GetById(criteria);

                if (team != null)
                {
                    if (team.RawLeader != null)
                    {
                        var leader = new Employee
                        {
                            EmpId = team.RawLeader.LeaderId,
                            NickName = team.RawLeader.NickName,
                            FullName = team.RawLeader.FullName
                        };
                        team.Leader = leader;
                    }

                    if (team.RawAssistantLeader != null)
                    {
                        var assistantLeader = new Employee
                        {
                            EmpId = team.RawAssistantLeader.AssistantLeaderId,
                            NickName = team.RawAssistantLeader.NickName,
                            FullName = team.RawAssistantLeader.FullName
                        };
                        team.AssistantLeader = assistantLeader;
                    }
                }
            }

            return team;
        }

        /// <summary>
        /// Get all team.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The list Team.</returns>
        public IList<Team> GetAll(GetAllGroupCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<Team> allTeam = null;

            if (criteria.IsValid())
            {
                allTeam = _teamRepository.GetAll(criteria);
            }

            return allTeam;
        }

        /// <summary>
        /// Add new Team.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The Command Result.</returns>
        public CommandResult AddNewTeam(InsertNewGroupCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult commandResult = new CommandResult();

            if (criteria.IsValid())
            {
                commandResult = Repository.Insert(criteria);
            }

            return commandResult;
        }

        /// <summary>
        /// Update Team.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The Command Result.</returns>
        public CommandResult UpdateTeam(UpdateGroupCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult commandResult = new CommandResult();

            if (criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        /// <summary>
        /// Delete Team.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The Command Result.</returns>
        public CommandResult DeleteTeam(DeleteGroupCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult commandResult = new CommandResult();

            if (criteria.IsValid())
            {
                commandResult = Repository.Delete(criteria);
            }

            return commandResult;
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

        public IList<TeamSupervisorModel> GetSupervisorByTeamId(GetSupervisorByTeamIdCriteria criteria)
        {
            IList<TeamSupervisorModel> supervisors = null;
            if (criteria.IsValid())
            {
                supervisors = _teamRepository.GetSupervisorByTeamId(criteria);
            }

            return supervisors;
        }

        public bool UpdateSupervisor(InsertOrUpdateTeamSupervisorCriteria criteria)
        {
            bool updated = false;
            if (criteria.IsValid())
            {
                updated = _teamRepository.UpdateSupervisor(criteria);
            }

            return updated;
        }

        public bool InsertTeamShiftManager(InsertTeamShiftManagerCriteria criteria)
        {
            bool inserted = false;
            if (criteria.IsValid())
            {
                inserted = _teamRepository.InsertTeamShiftManager(criteria);
            }

            return inserted;
        }
    }
}