namespace Fanex.NexHR.ViewModels.EmployeeGroup
{
    using System;
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

        [Display(Name = "Leader", ResourceType = typeof(Labels))]
        public IList<Employee> Leaders { get; set; }

        [Required]
        public string LeaderIds { get; set; }

        [Display(Name = "Assistant Leader")]
        public IList<Employee> AssistantLeaders { get; set; } = new List<Employee>();

        public string AssistantLeaderIds { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; }

        public int CreatedBy { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime ModifiedDate { get; set; }

        public int ModifiedBy { get; set; }

        public IList<Department> Departments { get; set; }

        public IList<Employee> Employees { get; set; }
    }
}