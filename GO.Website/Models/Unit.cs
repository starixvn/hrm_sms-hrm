namespace GO.Website.Models
{
    using System;

    [Serializable]
    public class Unit
    {
        public short UnitId { get; set; }
        public string Name { get; set; }
        public bool HasFlow { get; set; }
    }
}