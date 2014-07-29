// --------------------------------------------------------------------------
// <copyright file="GetUserLoginByCodeCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.7.25">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Employee
{
    using Fanex.Data.Repository;

    /// <summary>
    /// Get User Login By Code Criteria.
    /// </summary>
    public class GetUserLoginByCodeCriteria : CriteriaBase
    {
        public string Code { get; set; }

        public override string GetSettingKey()
        {
            return "Employee_GetEmployeeByCode";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(Code);
        }
    }
}
