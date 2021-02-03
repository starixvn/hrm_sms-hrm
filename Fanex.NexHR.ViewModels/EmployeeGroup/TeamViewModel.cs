namespace Fanex.NexHR.ViewModels.EmployeeGroup
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Group;
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
        [Display(Name = nameof(Labels.Section), ResourceType = typeof(Labels))]
        public int? SectionId { get; set; }

        [EmailAddress(ErrorMessage = "{0} is invalid.")]
        [StringLength(50, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = nameof(Messages.StringLengthErrorMessage))]
        [Display(Name = nameof(Labels.Email), ResourceType = typeof(Labels))]
        public string MailGroup { get; set; }

        public IEnumerable<TeamManager> Managers { get; set; } = new List<TeamManager>();

        [Required(ErrorMessageResourceName = nameof(Messages.RequiredErrorMessage), ErrorMessageResourceType = typeof(Messages))]
        [Display(Name = nameof(Labels.UnitHead), ResourceType = typeof(Labels))]
        public int? HeadId { get; set; }

        [Display(Name = nameof(Labels.UnitHead), ResourceType = typeof(Labels))]
        public string UnitHead { get; set; }

        [Display(Name = nameof(Labels.Manager), ResourceType = typeof(Labels))]
        public string ManagerIds { get; set; }

        [Display(Name = nameof(Labels.Supervisor), ResourceType = typeof(Labels))]
        public string SupervisorIds { get; set; }

        [Display(Name = nameof(Labels.TeamLeader), ResourceType = typeof(Labels))]
        public string LeaderIds { get; set; }

        public IList<Section> Sections { get; set; }

        public IList<Employee> Employees { get; set; }

        [Required(ErrorMessageResourceName = nameof(Messages.RequiredErrorMessage), ErrorMessageResourceType = typeof(Messages))]
        [Display(Name = nameof(Labels.BasicAnnualLeaveBalance), ResourceType = typeof(Labels))]
        public byte? LocalBasicAnnualLeaveBalance { get; set; }

        [Required(ErrorMessageResourceName = nameof(Messages.RequiredErrorMessage), ErrorMessageResourceType = typeof(Messages))]
        [Display(Name = nameof(Labels.WorkingDaysPerMonth), ResourceType = typeof(Labels))]
        public byte? LocalWorkingDays { get; set; }

        [Required(ErrorMessageResourceName = nameof(Messages.RequiredErrorMessage), ErrorMessageResourceType = typeof(Messages))]
        [Display(Name = nameof(Labels.WorkingHoursPerDay), ResourceType = typeof(Labels))]
        public byte? LocalWorkingHours { get; set; }

        [Required(ErrorMessageResourceName = nameof(Messages.RequiredErrorMessage), ErrorMessageResourceType = typeof(Messages))]
        [Display(Name = nameof(Labels.NumberOfOffday), ResourceType = typeof(Labels))]
        public byte? LocalOffDaysbyWeek { get; set; }

        [Required(ErrorMessageResourceName = nameof(Messages.RequiredErrorMessage), ErrorMessageResourceType = typeof(Messages))]
        [Display(Name = nameof(Labels.BasicAnnualLeaveBalance), ResourceType = typeof(Labels))]
        public byte? ExpatBasicAnnualLeaveBalance { get; set; }

        [Required(ErrorMessageResourceName = nameof(Messages.RequiredErrorMessage), ErrorMessageResourceType = typeof(Messages))]
        [Display(Name = nameof(Labels.WorkingDaysPerMonth), ResourceType = typeof(Labels))]
        public byte? ExpatWorkingDays { get; set; }

        [Required(ErrorMessageResourceName = nameof(Messages.RequiredErrorMessage), ErrorMessageResourceType = typeof(Messages))]
        [Display(Name = nameof(Labels.WorkingHoursPerDay), ResourceType = typeof(Labels))]
        public byte? ExpatWorkingHours { get; set; }

        [Required(ErrorMessageResourceName = nameof(Messages.RequiredErrorMessage), ErrorMessageResourceType = typeof(Messages))]
        [Display(Name = nameof(Labels.NumberOfOffday), ResourceType = typeof(Labels))]
        public byte? ExpatOffDaysbyWeek { get; set; }
    }
}