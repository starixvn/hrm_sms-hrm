// -----------------------------------------------------------------------
// <copyright file="DeleteMessageByEmployeeCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="2014.7.18">Created.</change>
// </history>

namespace Fanex.NexHR.Criteria.Message
{
    using Fanex.Data.Repository;

    /// <summary>
    /// Delete Message by employee criteria.
    /// </summary>
    public class DeleteMessageByEmployeeCriteria : CriteriaBase
    {
        public int MessageId { get; set; }

        public int EmployeeId { get; set; }

        public override string GetSettingKey()
        {
            return "Message_EmployeeDelete";
        }

        public override bool IsValid()
        {
            return MessageId > 0 && EmployeeId > 0;
        }
    }
}
