// -----------------------------------------------------------------------
// <copyright file="GetMessageByCreatorCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="2014.7.15">Created.</change>
// </history>

namespace Fanex.NexHR.Criteria.Message
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// Get message by creator criteria.
    /// </summary>
    public class GetMessageByCreatorCriteria : CriteriaBase
    {
        public int CreatedBy { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public bool? IsDisabled { get; set; }

        public override string GetSettingKey()
        {
            return "Message_GetByCreator";
        }

        public override bool IsValid()
        {
            return CreatedBy > 0;
        }
    }
}
