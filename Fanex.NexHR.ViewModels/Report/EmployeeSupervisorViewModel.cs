namespace Fanex.NexHR.ViewModels
{
    using System.Collections.Generic;
    using Fanex.NexHR.Models;

    public class EmployeeSupervisorViewModel
    {
        public int? EmployeeId { get; set; }

        public string GroupIds { get; set; }

        public int? ActionType { get; set; }

        public byte? IsLocal { get; set; }

        public IList<Team> Groups { get; set; }
    }
}