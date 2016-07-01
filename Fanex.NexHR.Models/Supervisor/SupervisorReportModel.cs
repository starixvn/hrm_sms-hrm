namespace Fanex.NexHR.Models.Supervisor
{
    using System.Collections.Generic;

    public class SupervisorReportModel : Employee
    {
        public SupervisorReportModel()
        {
            Supervisors = new List<SupervisorModel>();
        }

        public IList<SupervisorModel> Supervisors { get; set; }
    }
}