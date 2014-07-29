// --------------------------------------------------------------------------
// <copyright file="RequestRelatedPeopleModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2014.7.29">Created</change>
// </history>

namespace Fanex.NexHR.Models.Request
{
    using System.Collections.Generic;

    /// <summary>
    /// RequestRelated People.
    /// </summary>
    public class RequestRelatedPeopleModel
    {
        public SupervisorNameModel Requester { get; set; }

        public SupervisorNameModel Verifier { get; set; }

        public SupervisorNameModel Approver { get; set; }

        public IList<SupervisorNameModel> ListHR { get; set; }

        public EmailSettingsModel EmailSetting { get; set; }
    }
}
