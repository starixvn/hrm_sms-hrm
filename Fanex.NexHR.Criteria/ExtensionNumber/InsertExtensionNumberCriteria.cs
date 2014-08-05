// -----------------------------------------------------------------------
// <copyright file="InsertExtensionNumberCriteria.cs" company="Nexcel Solutions Vietnam">
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
    /// Insert extension number criteria.
    /// </summary>
    public class InsertExtensionNumberCriteria : CriteriaBase
    {
        public string FullName { get; set; }

        public string NickName { get; set; }

        public string ExtensionNo { get; set; }

        public bool IsInternal { get; set; }

        public int EmpID { get; set; }

        public int GroupID { get; set; }

        public short OrderNo { get; set; }

        public override string GetSettingKey()
        {
            return "ExtensionNO_Insert_EmployeeInfo";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
