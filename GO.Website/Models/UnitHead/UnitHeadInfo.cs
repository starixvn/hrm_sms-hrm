namespace GO.Website.Models.UnitHead
{
    using System;

    [Serializable]
    public class UnitHeadInfo
    {
        public short SiteId { get; set; }
        public int EmpId { get; set; }
        public string Code { get; set; }
        public string Nickname { get; set; }
        public string FullName { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
    }
}