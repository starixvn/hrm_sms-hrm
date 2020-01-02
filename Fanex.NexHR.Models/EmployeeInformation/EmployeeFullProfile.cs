namespace Fanex.NexHR.Models.EmployeeInformation
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Core.Enums;

    public class EmployeeFullProfile
    {
        public int EmpId { get; set; }

        public string Code { get; set; }

        public string FullName { get; set; }

        public string NickName { get; set; }

        public string ChineseName { get; set; }

        public DateTime DateOfBirth { get; set; }

        public byte? Gender { get; set; }

        public byte Marriage { get; set; }

        public string MarriageName
        {
            get
            {
                return ((MarriageStatus)Marriage).ToString();
            }
        }

        public string CurrentAddress { get; set; }

        public string Address { get; set; }

        public string CardNo { get; set; }

        public DateTime? DateOfIdCard { get; set; }

        public string PlaceOfIdCard { get; set; }

        public string PlaceOfBirth { get; set; }

        public string HomePhone { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string Qualification { get; set; }

        public string Major { get; set; }

        public string GroupName { get; set; }
        public EmployeeEmergency Emergency { get; set; }

        public IEnumerable<EmployeeRelationship> Relationships { get; set; }
    }
}