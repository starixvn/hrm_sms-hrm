// --------------------------------------------------------------------------
// <copyright file="GetEmployeeMissingRequestDateCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.7.29">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.MySpace
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// Get Employee Missing Request Date Criteria.
    /// </summary>
    public class GetEmployeeMissingRequestDateCriteria : CriteriaBase
    {
        public int EmpId { get; set; }

        public DateTime Date { get; set; }

        public override string GetSettingKey()
        {
            return "Report_DateOfEmployeeForgotAndLateEarlyIO";
        }

        public override bool IsValid()
        {
            return EmpId > 0;
        }
    }
}
