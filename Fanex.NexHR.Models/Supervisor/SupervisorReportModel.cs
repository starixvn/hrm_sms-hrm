namespace Fanex.NexHR.Models.Supervisor
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.NexHR.Core.Enums;
    using Nexus.RoleManagement.Services;

    public class SupervisorReportModel
    {
        public SupervisorReportModel()
        {
            Supervisors = new List<SupervisorModel>();
        }

        public int Id { get; set; }

        public string NickName { get; set; }

        public string FullName { get; set; }

        public string Code { get; set; }

        public List<SystemRoleModel> Roles { get; set; }

        public string Roles2String
        {
            get
            {
                return Roles == null ? string.Empty : string.Join(",", Roles.Select(i => i.RoleName));
            }
        }

        public SectionModel Section { get; set; }

        public IList<SupervisorModel> Supervisors { get; set; }
    }
}