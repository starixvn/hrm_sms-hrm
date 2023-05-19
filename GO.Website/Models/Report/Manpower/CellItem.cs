namespace GO.Website.Models.Report.Manpower
{
    using System;

    [Serializable]
    public class CellItem
    {
        public string Name { get; set; }
        public int Total { get; set; }
        public int TotalLocal { get; set; }
        public int TotalExpat { get; set; }
    }
}