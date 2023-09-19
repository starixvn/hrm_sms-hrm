namespace GO.Website.Models.Report.Manpower
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class GridData
    {
        public IList<RowItem> Headers { get; set; }
        public IList<RowItem> Supervisors { get; set; }
        public IList<RowItem> Employees { get; set; }
        public IList<RowItem> TotalJobTypeSupervisor { get; set; }
        public IList<RowItem> TotalJobTypeEmployee { get; set; }
        public RowItem TotalSupervisor { get; set; }
        public RowItem TotalEmployee { get; set; }
        public RowItem GrandTotal { get; set; }
    }
}