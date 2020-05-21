namespace Fanex.NexHR.Models
{
    public class NewDepartment : EntityBase<int>
    {
        public string Unit { get; set; }

        public bool IsActive { get; set; }
    }
}
