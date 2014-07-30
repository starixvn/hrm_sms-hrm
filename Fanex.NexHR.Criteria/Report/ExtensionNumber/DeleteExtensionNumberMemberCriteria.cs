// -----------------------------------------------------------------------
// <copyright file="DeleteExtensionNumberMemberCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="2014.7.30">Created.</change>
// </history>

namespace Fanex.NexHR.Criteria.Report.ExtensionNumber
{
    using Fanex.Data.Repository;

    /// <summary>
    /// Delete extension number of member criteria.
    /// </summary>
    public class DeleteExtensionNumberMemberCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ExtID")]
        public int ExtensionNumberId { get; set; }

        public override string GetSettingKey()
        {
            return "ExtensionNO_DELETE_EmployeeInfo";
        }

        public override bool IsValid()
        {
            return ExtensionNumberId > 0;
        }
    }
}
