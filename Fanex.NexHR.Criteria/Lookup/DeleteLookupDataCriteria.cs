// -----------------------------------------------------------------------
// <copyright file="DeleteLookupDataCriteria.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="2014.7.31">Created.</change>
// </history>

namespace Fanex.NexHR.Criteria.Lookup
{
    using Fanex.Data.Repository;

    /// <summary>
    /// Delete lookup data criteria.
    /// </summary>
    public class DeleteLookupDataCriteria : CriteriaBase
    {
        public int Id { get; set; }

        public override string GetSettingKey()
        {
            return "LookupData_Delete";
        }

        public override bool IsValid()
        {
            return Id > 0;
        }
    }
}
