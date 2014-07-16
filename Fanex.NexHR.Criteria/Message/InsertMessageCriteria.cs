// -----------------------------------------------------------------------
// <copyright file="InsertMessageCriteria.cs" company="Nexcel Solutions Vietnam">
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
    /// Insert message criteria.
    /// </summary>
    public class InsertMessageCriteria : CriteriaBase
    {
        public string Subject { get; set; }

        public string Contents { get; set; }

        public int Category { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public string EmployeeIds { get; set; }

        public string GroupIds { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public override string GetSettingKey()
        {
            return "Message_Insert";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
