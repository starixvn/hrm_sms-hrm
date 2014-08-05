// -----------------------------------------------------------------------
// <copyright file="UpdateExternalNumberCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="2014.7.30">Created.</change>
// </history>

namespace Fanex.NexHR.Criteria.ExtensionNumber
{
    using Fanex.Data.Repository;

    /// <summary>
    /// Update external number criteria.
    /// </summary>
    public class UpdateExternalNumberCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ExtID")]
        public int ExternalId { get; set; }

        public string FullName { get; set; }

        public override string GetSettingKey()
        {
            return "ExtensionNO_Update_GroupExternal";
        }

        public override bool IsValid()
        {
            return ExternalId > 0;
        }
    }
}
