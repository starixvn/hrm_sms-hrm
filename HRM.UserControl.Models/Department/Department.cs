namespace HRM.UserControl.Models.Department
{
    using System;
    using HRM.UserControl.Extensions.XML;

    [Serializable]
    [XmlName("Row")]
    public class Department
    {
        public int DepartmentId { get; set; }
        public int UnitId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}