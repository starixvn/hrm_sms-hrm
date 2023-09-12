namespace GO.Website.Models.Manpower
{
    using System;

    [Serializable]
    public class Section
    {
        public string SecTionName { get; set; }
        public string DepartmentName { get; set; }
        public int DepartmentId { get; set; }
        public int SectionId { get; set; }
    }
}
