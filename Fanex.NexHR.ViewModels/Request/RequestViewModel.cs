namespace Fanex.NexHR.ViewModels.Request
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Resources;

    public class RequestViewModel
    {
        private bool isFilterByAffectedDate;

        public RequestViewModel()
        {
            StatusId = -2;
            FromDate = DateTime.Now.Date;
            ToDate = DateTime.Now.Date;
            DateOption = 0;
            TeamIds = null;
            EmployeeId = -1;
            RequestType = (int)RequestFilter.All;
        }

        public int DateOption { get; set; }

        [Display(Name = "Status")]
        public int StatusId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FromDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ToDate { get; set; }

        public int EmployeeId { get; set; }

        public string Employee { get; set; }

        public string TeamIds { get; set; }

        public string Team { get; set; }

        public int RequestType { get; set; }

        [Display(Name = "Team")]
        public IList<Team> TeamCollection { get; set; }

        public byte? IsLocal { get; set; }

        public int CurrentUserId { get; set; }

        public RequestViewType ViewType { get; set; }

        public string ActionName
        {
            get
            {
                var action = string.Empty;
                switch (ViewType)
                {
                    case RequestViewType.PersonalSpace:
                        action = "Member";
                        break;

                    case RequestViewType.HRSpace:
                        action = "HRView";
                        break;

                    case RequestViewType.CompanySpace:
                        action = "DailyView";
                        break;

                    case RequestViewType.SASpace:
                        action = "VoidRequest";
                        break;
                }

                return action;
            }
        }

        public bool IsAllowChangeStatus
        {
            get
            {
                return ViewType != RequestViewType.CompanySpace;
            }
        }

        public bool IsFilterByAffectedDate
        {
            get
            {
                if (StatusId == -2)
                {
                    isFilterByAffectedDate = ViewType != RequestViewType.PersonalSpace;
                }

                return isFilterByAffectedDate;
            }

            set
            {
                isFilterByAffectedDate = value;
            }
        }

        public IList<RequestEntity> Requests { get; set; }

        public IList<SelectListItem> GetSelecListItemTeam
        {
            get
            {
                IList<SelectListItem> selecListItemTeam = new List<SelectListItem>();

                if (TeamCollection == null)
                {
                    TeamCollection = new List<Team>();
                }

                var teamIds = TeamIds != null ? TeamIds.Split(',') : null;

                foreach (Team team in TeamCollection)
                {
                    var isSelected = false;
                    if (teamIds != null)
                    {
                        int pos = Array.IndexOf(teamIds, team.Id.ToString());
                        if (pos > -1)
                        {
                            isSelected = true;
                        }
                    }

                    var item = new SelectListItem
                    {
                        Value = team.Id.ToString(),
                        Text = team.Name,
                        Selected = isSelected
                    };

                    selecListItemTeam.Add(item);
                }

                return selecListItemTeam;
            }
        }

        public string BreadcrumbText
        {
            get
            {
                return Enumerations.GetStringValue(ViewType);
            }
        }

        public string PageTitle
        {
            get
            {
                var pageTitle = string.Empty;
                switch (ViewType)
                {
                    case RequestViewType.PersonalSpace:
                        pageTitle = Labels.MemberRequest;
                        break;

                    case RequestViewType.HRSpace:
                        pageTitle = Labels.EmployeeRequest;
                        break;

                    case RequestViewType.CompanySpace:
                        pageTitle = Labels.DailyRequests;
                        break;

                    case RequestViewType.SASpace:
                        pageTitle = Labels.VoidRequest;
                        break;
                }

                return pageTitle;
            }
        }
    }
}
