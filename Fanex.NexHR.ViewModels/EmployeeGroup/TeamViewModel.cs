namespace Fanex.NexHR.ViewModels.EmployeeGroup
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Section;
    using Fanex.NexHR.Resources;

#pragma warning disable S109 // Magic numbers should not be used
    public class TeamViewModel
    {
        public int Id { get; set; }

        [AllowHtml]
        [Required(ErrorMessageResourceName = nameof(Messages.RequiredErrorMessage), ErrorMessageResourceType = typeof(Messages))]
        [StringLength(20, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = nameof(Messages.StringLengthErrorMessage))]
        [Display(Name = nameof(Labels.Code), ResourceType = typeof(Labels))]
        public string TeamCode { get; set; }

        [AllowHtml]
        [Required(ErrorMessageResourceName = nameof(Messages.RequiredErrorMessage), ErrorMessageResourceType = typeof(Messages))]
        [StringLength(150, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = nameof(Messages.StringLengthErrorMessage))]
        [Display(Name = nameof(Labels.Name), ResourceType = typeof(Labels))]
        public string TeamName { get; set; }

        [Required(ErrorMessageResourceName = nameof(Messages.RequiredErrorMessage), ErrorMessageResourceType = typeof(Messages))]
        [Display(Name = nameof(Labels.Division), ResourceType = typeof(Labels))]
        public int? DepartmentId { get; set; }

        [EmailAddress(ErrorMessage = "{0} is invalid.")]
        [StringLength(50, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = nameof(Messages.StringLengthErrorMessage))]
        [Display(Name = nameof(Labels.Email), ResourceType = typeof(Labels))]
        public string MailGroup { get; set; }

        public IEnumerable<SectionManager> Managers { get; set; } = new List<SectionManager>();

        [Required(ErrorMessageResourceName = nameof(Messages.RequiredErrorMessage), ErrorMessageResourceType = typeof(Messages))]
        [Display(Name = nameof(Labels.SectionHead), ResourceType = typeof(Labels))]
        public int? HeadId { get; set; }

        [Display(Name = nameof(Labels.SectionHead), ResourceType = typeof(Labels))]
        public string SectionHead { get; set; }

        [Display(Name = nameof(Labels.AssistantSectionHead), ResourceType = typeof(Labels))]
        public string AssistantHeadIds { get; set; }

        [Display(Name = nameof(Labels.Leader), ResourceType = typeof(Labels))]
        public string LeaderIds { get; set; }

        [Display(Name = nameof(Labels.AssistantLeader), ResourceType = typeof(Labels))]
        public string AssistantLeaderIds { get; set; }

        [Required(ErrorMessageResourceName = nameof(Messages.RequiredErrorMessage), ErrorMessageResourceType = typeof(Messages))]
        [Display(Name = nameof(Labels.BasicAnnualLeaveBalance), ResourceType = typeof(Labels))]
        public byte? BasicAnnualLeaveBalance { get; set; }

        public IList<Department> Departments { get; set; }

        public IList<Employee> Employees { get; set; }
    }
}