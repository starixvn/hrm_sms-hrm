namespace Fanex.NexHR.ViewModels.EmployeeGroup
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Attributes;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Resources;

    public class TeamViewModel
    {
        public int Id { get; set; }

        [AllowHtml]
        [Required]
        [Display(Name = "Code", ResourceType = typeof(Labels))]
        public string TeamCode { get; set; }

        [AllowHtml]
        [Required]
        [Display(Name = "Name", ResourceType = typeof(Labels))]
        public string TeamName { get; set; }

        [Required]
        public int DepartmentId { get; set; }

        [Email]
        public string MailGroup { get; set; }

        [Display(Name = nameof(Labels.Leader), ResourceType = typeof(Labels))]
        public IList<TeamLeader> Leaders { get; set; } = new List<TeamLeader>();

        [Required(ErrorMessage = "Leaders field is required.")]
        public string LeaderIds { get; set; }

        [Display(Name = nameof(Labels.AssistantLeader), ResourceType = typeof(Labels))]
        public IList<TeamLeader> AssistantLeaders { get; set; } = new List<TeamLeader>();

        public string AssistantLeaderIds { get; set; }

        [Required]
        [Display(Name = nameof(Labels.BasicAnnualLeaveBalance), ResourceType = typeof(Labels))]
        public byte? BasicAnnualLeaveBalance { get; set; }

        public IList<Department> Departments { get; set; }

        public IList<Employee> Employees { get; set; }
    }
}