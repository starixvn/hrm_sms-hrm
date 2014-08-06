// --------------------------------------------------------------------------
// <copyright file="ITeamRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Repositories.Interfaces
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Supervisor;

    public interface ITeamRepository
    {
        Team GetById(ICriteria criteria);

        IList<Team> GetAll(ICriteria criteria);

        IList<SupervisorModel> GetTeamSupervisor(ICriteria criteria);

        IList<TeamSupervisorModel> GetSupervisorByTeamId(ICriteria criteria);

        bool UpdateSupervisor(ICriteria criteria);

        bool InsertTeamShiftManager(ICriteria criteria);
    }
}
