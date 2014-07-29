// -----------------------------------------------------------------------
// <copyright file="GetUnreadMessageOfEmployeeCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="2014.7.29">Created.</change>
// </history>

namespace Fanex.NexHR.Criteria.Message
{
    using Fanex.Data.Repository;

    /// <summary>
    /// Get unread message of employee criteria.
    /// </summary>
    public class GetUnreadMessageOfEmployeeCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public override string GetSettingKey()
        {
            return "Message_GetUnreadMessageByEmployee";
        }

        public override bool IsValid()
        {
            return EmployeeId > 0;
        }
    }
}
