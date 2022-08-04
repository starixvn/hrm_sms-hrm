namespace GO.Website.Models
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class SiteMenu
    {
        public short SiteId { get; set; }
        public string SiteName { get; set; }
        public string SiteURL { get; set; }
        public IList<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
    }
}