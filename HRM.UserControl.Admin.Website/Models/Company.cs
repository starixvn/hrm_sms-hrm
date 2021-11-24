namespace HRM.UserControl.Admin.Website.Models
{
    using System;

    [Serializable]
    public class Company
    {
        public short CompanyId { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }
}