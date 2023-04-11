namespace GO.Website.Models.Todolist
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class TodolistMetadata
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public long RequestId { get; set; }
        public byte RequestTypeId { get; set; }
        public byte StatusId { get; set; }
        public int EmpId { get; set; }
        public string Comment { get; set; }

        public IList<TodolistDiscipline> Disciplines { get; set; } = new List<TodolistDiscipline>();
    }
}