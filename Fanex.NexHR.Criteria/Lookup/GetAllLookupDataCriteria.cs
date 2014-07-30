// -----------------------------------------------------------------------
// <copyright file="GetAllLookupDataCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="2014.7.30">Created.</change>
// </history>

namespace Fanex.NexHR.Criteria.Lookup
{
    using Fanex.Data.Repository;

    /// <summary>
    /// Get all job title criteria.
    /// </summary>
    public class GetAllLookupDataCriteria : CriteriaBase
    {
        public string LookupCode { get; set; }

        public override string GetSettingKey()
        {
            return "Lookup_GetLookupData";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
