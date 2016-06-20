namespace Fanex.NexHR.ViewModels
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.NexHR.Models;

    public class UserManagementViewModel : PagerOptionModel
    {
        public string Keyword { get; set; }

        public string GroupIds { get; set; }

        public int? JobTitleId { get; set; }

        public bool? IsEnable { get; set; }

        public bool? IsDelete { get; set; }

        public bool? IsLocal { get; set; }

        public IList<Team> Groups { get; set; }

        public IList<SelectListItem> JobTitles { get; set; }
    }
}