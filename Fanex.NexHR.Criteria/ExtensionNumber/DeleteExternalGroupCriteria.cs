// -----------------------------------------------------------------------
// <copyright file="DeleteExternalGroupCriteria.cs" company="Nexcel Solutions Vietnam">
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
    /// Delete external group criteria.
    /// </summary>
    public class DeleteExternalGroupCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ExtID")]
        public int ExternalId { get; set; }

        public override string GetSettingKey()
        {
            return "ExtensionNO_DELETE_GroupExternal";
        }

        public override bool IsValid()
        {
            return ExternalId > 0;
        }
    }
}
