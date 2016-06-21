// --------------------------------------------------------------------------
// <copyright file="UpdateEmployeePersonalInformationCriteria.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.7.29">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Employee
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// Update Employee Personal Information Criteria.
    /// </summary>
    public class UpdateEmployeePersonalInformationCriteria : CriteriaBase
    {
        public int EmpId { get; set; }

        [SpParam(SqlParamName = "DOB")]
        public DateTime DateOfBirth { get; set; }

        public string Phone { get; set; }

        public byte? Gender { get; set; }

        public string Skype { get; set; }

        public string Yahoo { get; set; }

        public byte Marriage { get; set; }

        public string PermanentAddress { get; set; }

        public string CurrentAddress { get; set; }

        public string IdCardNumber { get; set; }

        public string PassportNo { get; set; }

        public DateTime? PassportExp { get; set; }

        public string ChineseName { get; set; }

        public DateTime ModifiedDate { get; set; }

        public int ModifiedBy { get; set; }

        public string CountryCode { get; set; }

        public string PlaceOfbirth { get; set; }

        public string Ethnic { get; set; }

        public string Native { get; set; }

        public string Religion { get; set; }

        public DateTime? DateOfIDCard { get; set; }

        public string PlaceOfIDCard { get; set; }

        public string HomePhone { get; set; }

        public string Qualification { get; set; }

        public string Major { get; set; }

        public override string GetSettingKey()
        {
            return "Employee_Information_UpdateByEmpId";
        }

        public override bool IsValid()
        {
            return EmpId > 0;
        }
    }
}
