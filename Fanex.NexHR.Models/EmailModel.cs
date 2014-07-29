// --------------------------------------------------------------------------
// <copyright file="EmailModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2014.7.29">Created</change>
// </history>

namespace Fanex.NexHR.Models
{
    /// <summary>
    /// Email Model.
    /// </summary>
    public class EmailModel
    {
        public string Subject { get; set; }

        public string Body { get; set; }

        public string SendTo { get; set; }

        public string SendCc { get; set; }

        public string Sender { get; set; }

        public string SendToNickName { get; set; }

        public bool IsAllowSendRequester { get; set; }
    }
}
