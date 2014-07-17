// -----------------------------------------------------------------------
// <copyright file="GetMessageByEmployeeCriteria.cs" company="Nexcel Solutions Vietnam">
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
    /// Get Message by employee criteria.
    /// </summary>
    public class GetMessageByEmployeeCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public DateTime EffectedDate { get; set; }

        public override string GetSettingKey()
        {
            return "Message_GetByEmployee";
        }

        public override bool IsValid()
        {
            return EmployeeId > 0;
        }
    }
}
