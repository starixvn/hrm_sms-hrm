namespace Fanex.NexHR.ViewModels.EmployeeGroup
{
    using Fanex.NexHR.Core.Attributes;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Resources;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

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

        [Required]
        [Display(Name = "Leader", ResourceType = typeof(Labels))]
        public string Leader { get; set; }

        public int LeaderId { get; set; }

        [Display(Name = "Assistant Leader")]
        public string AssistantLeader { get; set; }

        public int? AssistantLeaderId { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; }

        public int CreatedBy { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime ModifiedDate { get; set; }

        public int ModifiedBy { get; set; }

        public IList<Department> Departments { get; set; }
    }
}
