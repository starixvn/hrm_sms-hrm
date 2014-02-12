// --------------------------------------------------------------------------
// <copyright file="ITeamService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Services.Interface
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.ViewModels;

    public interface ITeamService
    {
        Team Get(int teamId);

        IList<Team> GetAll();

        int AddNewTeam(TeamViewModel model);

        bool UpdateTeam(TeamViewModel model);

        bool DeleteTeam(int id, DateTime modifiedDate, int modifiedby, out int errCode, out string message);
    }
}