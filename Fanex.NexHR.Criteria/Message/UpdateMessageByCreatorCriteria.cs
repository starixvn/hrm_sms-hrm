// -----------------------------------------------------------------------
// <copyright file="UpdateMessageByCreatorCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="2014.7.17">Created.</change>
// </history>

namespace Fanex.NexHR.Criteria.Message
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// Update Message criteria.
    /// </summary>
    public class UpdateMessageByCreatorCriteria : CriteriaBase
    {
        public int MessageId { get; set; }

        public string Subject { get; set; }

        public string Contents { get; set; }

        public int Category { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public string EmployeeIds { get; set; }

        public string GroupIds { get; set; }

        [SpParam(Ignored = true)]
        public string SendToEmail { get; set; }

        public int ModifiedBy { get; set; }

        public DateTime ModifiedDate { get; set; }

        public override string GetSettingKey()
        {
            return "Message_Update";
        }

        public override bool IsValid()
        {
            return MessageId > 0;
        }
    }
}
