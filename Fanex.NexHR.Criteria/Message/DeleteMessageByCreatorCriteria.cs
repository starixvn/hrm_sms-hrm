// -----------------------------------------------------------------------
// <copyright file="DeleteMessageByCreatorCriteria.cs" company="Nexcel Solutions Vietnam">
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
    /// Delete Message criteria.
    /// </summary>
    public class DeleteMessageByCreatorCriteria : CriteriaBase
    {
        public int MessageId { get; set; }

        public int ModifiedBy { get; set; }

        public DateTime ModifiedDate { get; set; }

        public override string GetSettingKey()
        {
            return "Message_Delete";
        }

        public override bool IsValid()
        {
            return MessageId > 0;
        }
    }
}
