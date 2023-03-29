namespace GO.Website.Models.Todoist
{
    using System;

    [Serializable]
    public class TodoistAction
    {
        public string Id => Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string Url { get; set; }
        public string Method { get; set; }
        public TodoistMetadata Metadata { get; set; }
        public bool IsRedirect { get; set; }
    }
}