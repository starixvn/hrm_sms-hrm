namespace GO.Website.Models.Todoist
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class TodoistMetadata
    {
        public long RequestId { get; set; }
        public byte RequestTypeId { get; set; }
        public byte StatusId { get; set; }
        public int EmpId { get; set; }
        public string Code { get; set; }
        public string Nickname { get; set; }
        public string FullName { get; set; }
        public string Remark { get; set; }

        public IList<Discipline> Disciplines { get; set; } = new List<Discipline>();
    }
}