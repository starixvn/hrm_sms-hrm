namespace HRM.UserControl.Admin.Website.Models.Session
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class MenuItemSession
    {
        public int MenuId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Link { get; set; }

        public int? ParentId { get; set; }

        public int DisplayOrder { get; set; }

        public IEnumerable<MenuItemSession> Childs { get; set; } = new List<MenuItemSession>();
    }
}