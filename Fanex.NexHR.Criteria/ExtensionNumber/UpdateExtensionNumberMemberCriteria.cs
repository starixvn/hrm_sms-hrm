// -----------------------------------------------------------------------
// <copyright file="UpdateExtensionNumberMemberCriteria.cs" company="Nexcel Solutions Vietnam">
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
    /// Update extension number of member criteria.
    /// </summary>
    public class UpdateExtensionNumberMemberCriteria : CriteriaBase
    {
        public int ExtID { get; set; }

        public string FullName { get; set; }

        public string NickName { get; set; }

        public string ExtensionNo { get; set; }

        public bool IsInternal { get; set; }

        public int EmpID { get; set; }

        public int GroupID { get; set; }

        public short OrderNo { get; set; }

        public override string GetSettingKey()
        {
            return "ExtensionNO_Update_EmployeeInfo";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
