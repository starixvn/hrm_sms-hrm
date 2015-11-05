// -----------------------------------------------------------------------
// <copyright file="InsertLookupDataCriteria.cs" company="Nexcel Solutions Vietnam">
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
    /// Insert Lookup Data Criteria.
    /// </summary>
    public class InsertLookupDataCriteria : CriteriaBase
    {
        public string LookupCode { get; set; }

        public string Name { get; set; }

        public string CategoryId { get; set; }

        public override string GetSettingKey()
        {
            return "LookupData_Insert";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
