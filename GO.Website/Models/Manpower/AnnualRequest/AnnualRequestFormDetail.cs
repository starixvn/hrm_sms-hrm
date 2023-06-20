namespace GO.Website.Models.Manpower
{
    using System;

    [Serializable]
    public class AnnualRequestFormDetail
    {
        public short SiteId { get; set; }
        public string SiteName { get; set; }

        public short CompanyId { get; set; }
        public string CompanyName { get; set; }

        public int UnitId { get; set; }
        public string UnitName { get; set; }

        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }

        public int SectionId { get; set; }
        public string SectionName { get; set; }

        public string JobLevel { get; set; }

        public int LocalCurrentHeadcount { get; set; }
        public int ExpatCurrentHeadcount { get; set; }
        public int TotalCurrentHeadcount => LocalCurrentHeadcount + ExpatCurrentHeadcount;

        public int LocalApprovedHeadcount { get; set; }
        public int ExpatApprovedHeadcount { get; set; }
        public int TotalApprovedHeadcount => LocalApprovedHeadcount + ExpatApprovedHeadcount;

        public int LocalAddition { get; set; }
        public int ExpatAddition { get; set; }
        public int TotalAddition => LocalAddition + ExpatAddition;
    }
}