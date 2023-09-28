namespace GO.Website.Models.Manpower
{
    using System;

    [Serializable]
    public class EmployeeInfo
    {
        public string CompanyName { get; set; }
        public short SiteId { get; set; }
        public string SiteName { get; set; }
        public string Country { get; set; }
        public int EmpId { get; set; }
        public string Email { get; set; }
        public int UserId { get; set; }
        public int CreatorId { get; set; }
        public string Username { get; set; }
        public string Code { get; set; }
        public string Nickname { get; set; }
        public string FullName { get; set; }
        public string JobLevel { get; set; }
        public bool IsLocal { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsExcluded { get; set; }
        public bool IsOTPRegistered { get; set; }

        public string TeamCode { get; set; }
        public string TeamName { get; set; }
        public string SectionCode { get; set; }
        public string SectionName { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public string UnitCode { get; set; }
        public string UnitName { get; set; }

        public DateTime JoinedDate { get; set; }
        public DateTime ConfirmedDate { get; set; }
        public DateTime ResignedDate { get; set; }
    }
}