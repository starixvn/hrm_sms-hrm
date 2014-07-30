// -----------------------------------------------------------------------
// <copyright file="GetAllExtensionNumberCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="2014.7.30">Created.</change>
// </history>

namespace Fanex.NexHR.Criteria.Report.ExtensionNumber
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// Get all extension number criteria.
    /// </summary>
    public class GetAllExtensionNumberCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "ExtensionNO_GetAll";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
