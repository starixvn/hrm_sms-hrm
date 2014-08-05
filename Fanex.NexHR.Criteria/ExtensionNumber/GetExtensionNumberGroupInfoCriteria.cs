// -----------------------------------------------------------------------
// <copyright file="GetExtensionNumberGroupInfoCriteria.cs" company="Nexcel Solutions Vietnam">
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
    /// Get extension number group info criteria.
    /// </summary>
    public class GetExtensionNumberGroupInfoCriteria : CriteriaBase
    {
        public bool? IsInternal { get; set; }

        public int? GroupID { get; set; }

        public override string GetSettingKey()
        {
            return "ExtensionNO_GetGroupInfo";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
