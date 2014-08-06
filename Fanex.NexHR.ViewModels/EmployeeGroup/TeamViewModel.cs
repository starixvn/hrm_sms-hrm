// --------------------------------------------------------------------------
// <copyright file="TeamViewModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.ViewModels.EmployeeGroup
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Fanex.NexHR.Core.Attributes;
    using Fanex.NexHR.Resources;

    /// <summary>
    /// Team ViewModel.
    /// </summary>
    public class TeamViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Code", ResourceType = typeof(Labels))]
        public string TeamCode { get; set; }

        [Required]
        [Display(Name = "Name", ResourceType = typeof(Labels))]
        public string TeamName { get; set; }

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
    }
}
