using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Fanex.NexHR.Models;

namespace Fanex.NexHR.ViewModels.Report
{
    public class WeekendLeaveSummaryViewModel
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public IList<Team> Teams { get; set; }

        [Display(Name = "Team")]
        public int? TeamId { get; set; }

        public string TeamName { get; set; }

        public bool? IsLocal { get; set; }

        public string Employee { get; set; }

        public int? EmployeeId { get; set; }

        public bool FromSummaryPage { get; set; }

        public bool IsShowAll { get; set; }

        public IList<SelectListItem> DateRange { get; set; }
    }
}