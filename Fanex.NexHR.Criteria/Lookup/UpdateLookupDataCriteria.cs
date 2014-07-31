// -----------------------------------------------------------------------
// <copyright file="UpdateLookupDataCriteria.cs" company="Nexcel Solutions Vietnam">
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
    /// Update lookup data criteria.
    /// </summary>
    public class UpdateLookupDataCriteria : CriteriaBase
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public override string GetSettingKey()
        {
            return "LookupData_Update";
        }

        public override bool IsValid()
        {
            return Id > 0;
        }
    }
}
