// --------------------------------------------------------------------------
// <copyright file="GetCountryByCodeCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.7.28">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Country
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// Get Country By Code Criteria.
    /// </summary>
    public class GetCountryByCodeCriteria : CriteriaBase
    {
        public string CountryCode { get; set; }

        public override string GetSettingKey()
        {
            return "Country_GetByCode";
        }

        public override bool IsValid()
        {
            return CountryCode != string.Empty;
        }
    }
}
