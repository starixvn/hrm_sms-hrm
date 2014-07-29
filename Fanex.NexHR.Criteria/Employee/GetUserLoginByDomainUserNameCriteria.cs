// --------------------------------------------------------------------------
// <copyright file="GetUserLoginByDomainUserNameCriteria.cs" company="Nexcel Solutions Vietnam">
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
    /// Get User Login By Domain User Name Criteria.
    /// </summary>
    public class GetUserLoginByDomainUserNameCriteria : CriteriaBase
    {
        public string DomainUsername { get; set; }

        public override string GetSettingKey()
        {
            return "GetEmployeeByDomainUserName";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(DomainUsername);
        }
    }
}
