namespace GO.Website.Models.Report.Manpower
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class GroupCellItem
    {
        public string Name { get; set; }
        public IList<CellItem> Cells { get; set; }
    }
}