// -----------------------------------------------------------------------
// <copyright file="GetMessageOfEmployeeByIdCriteria.cs" company="Nexcel Solutions Vietnam">
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
    /// Get message by Id of employee criteria.
    /// </summary>
    public class GetMessageOfEmployeeByIdCriteria : CriteriaBase
    {
        public int MessageId { get; set; }

        public override string GetSettingKey()
        {
            return "Message_GetDetailByEmployee";
        }

        public override bool IsValid()
        {
            return MessageId > 0;
        }
    }
}
