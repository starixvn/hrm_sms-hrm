// --------------------------------------------------------------------------
// <copyright file="GetAllCountryCriteria.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.7.28">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Country
{
    using Fanex.Data.Repository;

    /// <summary>
    /// Get All Country Criteria.
    /// </summary>
    public class GetAllCountryCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "Country_GetAll";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
