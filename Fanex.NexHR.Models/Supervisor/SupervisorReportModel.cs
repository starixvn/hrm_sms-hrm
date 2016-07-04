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

        public string Roles { get; set; }

        public string Roles2String
        {
            get
            {
                IList<SystemRoleModel> roleList = new List<SystemRoleModel>();

                if (!string.IsNullOrEmpty(Roles))
                {
                    IList<string> roles = Roles.Split(',');
                    roleList = roles.Select(i => new SystemRoleModel
                    {
                        RoleId = int.Parse(i),
                        RoleCode = ((RoleSystem)int.Parse(i)).ToString(),
                        RoleName = ((RoleSystem)int.Parse(i)).ToString()
                    }).ToList();
                }

                return !roleList.Any() ? string.Empty : string.Join(", ", roleList.Select(i => i.RoleName));
            }
        }

        public SectionModel Section { get; set; }

        public IList<SupervisorModel> Supervisors { get; set; }
    }
}