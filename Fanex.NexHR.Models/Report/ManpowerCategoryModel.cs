namespace Fanex.NexHR.Models
{
    public class ManpowerCategoryModel
    {
        public string GroupCode { get; set; }

        public string GroupName { get; set; }

        public int Total { get; set; }

        public int TotalExpat { get; set; }

        public int TotalLocal { get; set; }

        public int Management { get; set; }

        public int ExecutiveWithPosition { get; set; }

        public int Executive { get; set; }

        public int Staff { get; set; }

        public int Worker { get; set; }
    }
}