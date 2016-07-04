namespace Fanex.NexHR.ViewModels
{
    using System.Collections.Generic;
    using Fanex.NexHR.Models;

    public class EmployeeAndSectionSupervisorViewModel
    {
        public int? EmployeeId { get; set; }

        public string SectionIds { get; set; }

        public int? ActionType { get; set; }

        public byte? IsLocal { get; set; }

        public IList<Team> Sections { get; set; }
    }
}