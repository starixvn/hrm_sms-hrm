// --------------------------------------------------------------------------
// <copyright file="EmailSettingsModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2014.2.13">Created</change>
// </history>

namespace Fanex.NexHR.Models
{
    /// <summary>
    /// EmailSettings Model.
    /// </summary>
    public class EmailSettingsModel
    {
        public int GroupId { get; set; }

        public int Action { get; set; }

        public bool AllowSendUser { get; set; }

        public bool AllowSendVerifier { get; set; }

        public bool AllowSendApprover { get; set; }

        public bool AllowSendHR { get; set; }

        public bool AllowSendMailGroup { get; set; }
    }
}
