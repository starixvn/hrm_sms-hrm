namespace Fanex.NexHR.Core.Models
{
    using System;

    [Serializable]
    public class MenuItemSession
    {
        public int MenuId { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public string Link { get; set; }
        public int? ParentId { get; set; }
        public int? DisplayOrder { get; set; }
        public bool IsActive { get; set; }
    }
}