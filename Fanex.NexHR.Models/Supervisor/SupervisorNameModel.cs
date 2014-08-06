// --------------------------------------------------------------------------
// <copyright file="SupervisorNameModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.8.6">Created</change>
// </history>

namespace Fanex.NexHR.Models.Supervisor
{
    /// <summary>
    /// Supervisor Name Model.
    /// </summary>
    public class SupervisorNameModel
    {
        public int OwnerId { get; set; }

        public string NickName { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string MailGroup { get; set; }

        public int ActionType { get; set; }
    }
}
