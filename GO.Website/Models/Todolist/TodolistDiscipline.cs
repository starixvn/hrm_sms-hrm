namespace GO.Website.Models.Todolist
{
    using System;

    [Serializable]
    public class TodolistDiscipline
    {
        public DateTime WorkDate { get; set; }
        public byte TypeId { get; set; }
        public string Name { get; set; }
        public string Details { get; set; }
        public bool IsIgnore { get; set; }
        public bool IsRecord { get; set; }
    }
}