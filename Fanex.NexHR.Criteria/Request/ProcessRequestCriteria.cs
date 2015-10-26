// --------------------------------------------------------------------------
// <copyright file="ProcessRequestCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2014.8.4">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Request
{
    using System;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Core.Enums;

    /// <summary>
    /// ProcessRequest Criteria.
    /// </summary>
    public class ProcessRequestCriteria : CriteriaBase
    {
        public long RequestId { get; set; }

        public int ProcessedBy { get; set; }

        public DateTime ProcessedDate { get; set; }

        public int RoleID { get; set; }

        public string Comment { get; set; }

        [SpParam(Ignored = true)]
        public string Action { get; set; }

        /// <summary>
        /// Function to get setting key.
        /// </summary>
        /// <returns>Setting key.</returns>
        public override string GetSettingKey()
        {
            var key = string.Empty;
            if (Action == Command.Accept.ToString()
                || Action == Command.Approve.ToString()
                || Action == Command.Process.ToString())
            {
                key = "WorkingTimeRequest_ApproveRequest";
            }
            else if (Action == Command.NotAccept.ToString()
                || Action == Command.Reject.ToString()
                || Action == Command.Void.ToString())
            {
                key = "WorkingTimeRequest_RejectRequest";
            }
            else if (Action == Command.Reset.ToString())
            {
                key = "WorkingTimeRequest_ResetRequest";
            }
            else
            {
                key = "WorkingTimeRequest_VoidRequest_bySA";
            }

            return key;
        }

        public override bool IsValid()
        {
            return RequestId > 0;
        }
    }
}
