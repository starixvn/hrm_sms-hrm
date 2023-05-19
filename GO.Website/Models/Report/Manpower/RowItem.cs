namespace GO.Website.Models.Report.Manpower
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class RowItem
    {
        public string Name { get; set; }
        public IList<GroupCellItem> Cells { get; set; }
    }
}