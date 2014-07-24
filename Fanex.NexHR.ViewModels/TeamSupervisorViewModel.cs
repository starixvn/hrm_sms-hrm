// --------------------------------------------------------------------------
// <copyright file="TeamSupervisorViewModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.7.23">Created</change>
// </history>

namespace Fanex.NexHR.ViewModels
{
    /// <summary>
    /// Team Supervisor ViewModel.
    /// </summary>
    public class TeamSupervisorViewModel
    {
        public int GroupId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public int ActionType { get; set; }

        public string Supervisor { get; set; }
    }
}
