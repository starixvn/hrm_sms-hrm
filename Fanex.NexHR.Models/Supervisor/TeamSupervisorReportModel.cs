namespace Fanex.NexHR.Models.Supervisor
{
    using System.Collections.Generic;

    public class SectionSupervisorReportModel
    {
        public SectionSupervisorReportModel()
        {
            Supervisors = new List<SupervisorModel>();
        }

        public int SectionId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public IList<SupervisorModel> Supervisors { get; set; }
    }
}