namespace HRM.UserControl.Client.Core.Models
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class MenuItem
    {
        public int MenuItemId { get; set; }
        public string MenuItemName { get; set; }
        public string URL { get; set; }
        public bool IsActive { get; set; }
        public int? ParentId { get; set; }
        public short DisplayOrder { get; set; }
        public IList<MenuItem> Childs { get; set; } = new List<MenuItem>();
    }
}