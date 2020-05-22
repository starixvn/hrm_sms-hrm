namespace Fanex.NexHR.Models
{
    public class Unit : EntityBase<int>
    {
        public UnitHead Manager { get; set; }

        public bool IsActive { get; set; }
    }
}