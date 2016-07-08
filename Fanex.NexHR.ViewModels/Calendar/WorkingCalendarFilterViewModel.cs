namespace Fanex.NexHR.ViewModels.Calendar
{
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;

    public class WorkingCalendarFilterViewModel
    {
        public WorkingCalendarFilterViewModel()
        {
            JobTitles = new List<SelectListItem>();
            Groups = new List<Team>();
        }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public IList<SelectListItem> JobTitles { get; set; }

        public IList<Team> Groups { get; set; }
    }
}
