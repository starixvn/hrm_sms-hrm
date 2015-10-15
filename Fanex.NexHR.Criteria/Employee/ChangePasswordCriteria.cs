// --------------------------------------------------------------------------
// <copyright file="ChangePasswordCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.7.30">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Employee
{
    using Fanex.Data.Repository;

    /// <summary>
    /// Change Password Criteria.
    /// </summary>
    public class ChangePasswordCriteria : CriteriaBase
    {
        public int EmpId { get; set; }

        public string Password { get; set; }

        [SpParam(Ignored = true)]
        public string NewPassword { get; set; }

        public bool PasswordReset { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "Employee_ChangePassword";
        }

        public override bool IsValid()
        {
            return EmpId > 0;
        }
    }
}
