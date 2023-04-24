namespace GO.Website.Models.Todolist
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class TodolistItem
    {
        public string Id => Guid.NewGuid().ToString();
        public short SiteId { get; set; }
        public string SiteName { get; set; }

        public string Name { get; set; }
        public short TypeId { get; set; }

        public IList<TodolistAction> Actions { get; set; } = new List<TodolistAction>();
    }
}