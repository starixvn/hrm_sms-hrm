namespace Fanex.NexHR.Models.Discipline
{
    public class ViolationModel
    {
        public int ViolationId { get; set; }

        public string Name { get; set; }

        public decimal? Score { get; set; }

        public int? Value { get; set; }

        public bool HasViolated { get; set; }

        public bool HasIgnored { get; set; }

        public bool HasApprovedBy { get; set; }
    }
}