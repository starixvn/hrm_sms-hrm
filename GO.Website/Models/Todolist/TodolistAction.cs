namespace GO.Website.Models.Todolist
{
    using System;

    [Serializable]
    public class TodolistAction
    {
        public string Id => Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string Url { get; set; }
        public string Method { get; set; }
        public TodolistMetadata Metadata { get; set; }
        public bool IsRedirect { get; set; }
    }
}