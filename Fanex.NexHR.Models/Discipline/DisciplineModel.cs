namespace Fanex.NexHR.Models.Discipline
{
    public class DisciplineModel
    {
        public int DisciplineId { get; set; }

        public string Name { get; set; }

        public decimal Score { get; set; }

        public string Value { get; set; }

        public bool HasViolated { get; set; }

        public bool HasIgnored { get; set; }

        public bool HasApprovedBy { get; set; }
    }
}