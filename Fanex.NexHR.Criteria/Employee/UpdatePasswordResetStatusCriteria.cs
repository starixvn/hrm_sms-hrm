// --------------------------------------------------------------------------
// <copyright file="UpdatePasswordResetStatusCriteria.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.7.30">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Employee
{
    using Fanex.Data.Repository;

    /// <summary>
    /// Update Password Reset Status Criteria.
    /// </summary>
    public class UpdatePasswordResetStatusCriteria : CriteriaBase
    {
        public int EmpId { get; set; }

        public bool PasswordReset { get; set; }

        public override string GetSettingKey()
        {
            return "Employee_UpdatePasswordResetStatusByEmpId";
        }

        public override bool IsValid()
        {
            return EmpId > 0;
        }
    }
}
