namespace Fanex.NexHR.Models
{
    public class NewDepartment : EntityBase<int>
    {
        public Unit Unit { get; set; }

        public bool IsActive { get; set; }
    }
}
