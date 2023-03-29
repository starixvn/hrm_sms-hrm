namespace GO.Website.Models.Todoist
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class TodoistItem
    {
        public string Id => Guid.NewGuid().ToString();
        public short SiteId { get; set; }
        public string Name { get; set; }
        public short TypeId { get; set; }

        public IList<TodoistAction> Actions { get; set; } = new List<TodoistAction>();
    }
}