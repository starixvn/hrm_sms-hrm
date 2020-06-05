namespace Fanex.NexHR.Models
{
    public class Section : EntityBase<int>
    {
        public NewDepartment Department { get; set; }

        public bool IsActive { get; set; }
    }
}