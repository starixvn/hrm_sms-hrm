// -----------------------------------------------------------------------
// <copyright file="GetJobTitleCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="2014.7.31">Created.</change>
// </history>

namespace Fanex.NexHR.Criteria.Lookup
{
    using Fanex.Data.Repository;

    /// <summary>
    /// Get Job Title criteria.
    /// </summary>
    public class GetJobTitleCriteria : CriteriaBase
    {
        public string JobTitle { get; set; }

        public override string GetSettingKey()
        {
            return "Lookup_GetJobTitle";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
