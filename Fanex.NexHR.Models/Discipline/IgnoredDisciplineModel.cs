namespace Fanex.NexHR.Models.Discipline
{
    using System;

    public class IgnoredDisciplineModel
    {
        public DateTime WorkDate { get; set; }

        public int EmployeeId { get; set; }

        public int DisciplineTypeId { get; set; }

        public bool Ignored { get; set; }
    }
}