namespace GO.Website.Models.Manpower
{
    using System;

    [Serializable]
    public class JobLevelMapping
    {
        public string Parent { get; set; }
        public string Child { get; set; }
    }
}