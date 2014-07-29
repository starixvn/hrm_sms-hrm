// --------------------------------------------------------------------------
// <copyright file="GetAnnualLeaveTransactionByIdCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.7.29">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.AnnualLeave
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fanex.Data.Repository;

    /// <summary>
    /// GetAnnualLeaveTransactionById Criteria.
    /// </summary>
    public class GetAnnualLeaveTransactionByIdCriteria : CriteriaBase
    {
        public int LeaveTransId { get; set; }

        public override string GetSettingKey()
        {
            return "AnnualLeave_GetByID";
        }

        public override bool IsValid()
        {
            return LeaveTransId > 0;
        }
    }
}
