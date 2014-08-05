// -----------------------------------------------------------------------
// <copyright file="GetExtensionNumberMemberByIdCriteria.cs" company="Nexcel Solutions Vietnam">
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
    /// Get extension number of member by Id criteria.
    /// </summary>
    public class GetExtensionNumberMemberByIdCriteria : CriteriaBase
    {
        public bool IsInternal { get; set; }

        [SpParam(SqlParamName = "EmpExtID")]
        public int EmployeeExtensionNoId { get; set; }

        public override string GetSettingKey()
        {
            return "ExtensionNO_GetEmployeeInfo";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
