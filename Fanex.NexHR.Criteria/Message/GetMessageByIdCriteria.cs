// -----------------------------------------------------------------------
// <copyright file="GetMessageByIdCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="2014.7.17">Created.</change>
// </history>

namespace Fanex.NexHR.Criteria.Message
{
    using Fanex.Data.Repository;

    /// <summary>
    /// Get Message by Id.
    /// </summary>
    public class GetMessageByIdCriteria : CriteriaBase
    {
        public int MessageId { get; set; }

        public override string GetSettingKey()
        {
            return "Message_GetById";
        }

        public override bool IsValid()
        {
            return MessageId > 0;
        }
    }
}
