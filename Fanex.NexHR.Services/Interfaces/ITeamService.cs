// --------------------------------------------------------------------------
// <copyright file="ITeamService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Services.Interfaces
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.GroupSupervisor;
    using Fanex.NexHR.Criteria.Team;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.ViewModels;

    public interface ITeamService
    {
        Team GetById(GetGroupByIdCriteria criteria);

        IList<Team> GetAll(GetAllGroupCriteria criteria);

        CommandResult AddNewTeam(InsertNewGroupCriteria criteria);

        CommandResult UpdateTeam(UpdateGroupCriteria criteria);

        CommandResult DeleteTeam(DeleteGroupCriteria criteria);

        IList<SupervisorModel> GetTeamSupervisor(GetTeamSupervisorCriteria criteria);

        IList<TeamSupervisorModel> GetSupervisorByTeamId(GetSupervisorByTeamIdCriteria criteria);

        bool UpdateSupervisor(InsertOrUpdateTeamSupervisorCriteria criteria);

        bool InsertTeamShiftManager(InsertTeamShiftManagerCriteria criteria);
    }
}