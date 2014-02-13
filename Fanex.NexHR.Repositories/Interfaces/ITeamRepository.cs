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
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.ViewModels;

    public interface ITeamRepository
    {
        Team GetById(int teamId);

        IList<Team> GetAll();

        int AddNewTeam(TeamViewModel model, out int errorCode, out string message);

        bool UpdateTeam(TeamViewModel model, out int errorCode, out string message);

        bool DeleteTeam(int id, DateTime modifiedDate, int modifiedBy, out int errorCode, out string message);
    }
}
