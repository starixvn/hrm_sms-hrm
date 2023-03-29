namespace GO.Website.Models.Todoist
{
    using System;

    [Serializable]
    public class Discipline
    {
        public byte TypeId { get; set; }
        public string Name { get; set; }
        public string Details { get; set; }
        public bool IsIgnore { get; set; }
    }
}