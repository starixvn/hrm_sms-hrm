// --------------------------------------------------------------------------
// <copyright file="GetNumberRequestInMonthCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2014.7.29">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Request
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// GetNumberRequestInMonth Criteria.
    /// </summary>
    public class GetNumberRequestInMonthCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public int RequestType { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public override string GetSettingKey()
        {
            return "WorkingTimeRequest_GetNumberRequestInMonth";
        }

        public override bool IsValid()
        {
            return RequestType > 0;
        }
    }
}
