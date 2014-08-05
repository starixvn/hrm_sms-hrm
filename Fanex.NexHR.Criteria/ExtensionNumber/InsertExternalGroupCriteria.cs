// -----------------------------------------------------------------------
// <copyright file="InsertExternalGroupCriteria.cs" company="Nexcel Solutions Vietnam">
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
    /// Insert external group criteria.
    /// </summary>
    public class InsertExternalGroupCriteria : CriteriaBase
    {
        public string FullName { get; set; }

        public override string GetSettingKey()
        {
            return "ExtensionNO_Insert_GroupExternal";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
