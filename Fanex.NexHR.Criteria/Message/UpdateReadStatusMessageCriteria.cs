// -----------------------------------------------------------------------
// <copyright file="UpdateReadStatusMessageCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="2014.7.28">Created.</change>
// </history>

namespace Fanex.NexHR.Criteria.Message
{
    using Fanex.Data.Repository;

    /// <summary>
    /// Update read status of message criteria.
    /// </summary>
    public class UpdateReadStatusMessageCriteria : CriteriaBase
    {
        public int MessageId { get; set; }

        public int EmployeeId { get; set; }

        public override string GetSettingKey()
        {
            return "Message_Read";
        }

        public override bool IsValid()
        {
            return MessageId > 0 && EmployeeId > 0;
        }
    }
}
