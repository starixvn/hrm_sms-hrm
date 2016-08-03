namespace Fanex.NexHR.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.NexHR.Models;

    public class OvertimeDetailReportViewModel
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int? EmployeeId { get; set; }

        public IList<Team> Sections { get; set; }

        public string SectionIds { get; set; }

        public IList<SelectListItem> JobTitles { get; set; }

        public string JobTitleIds { get; set; }

        public byte? IsLocal { get; set; }
    }
}