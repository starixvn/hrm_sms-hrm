namespace Fanex.NexHR.ViewModels.Request
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Helpers;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Resources;

    public class RequestViewModel
    {
        private bool isFilterByAffectedDate;

        public int DateOption { get; set; }

        [Display(Name = "Status")]
        public int StatusId { get; set; } = -2;

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FromDate { get; set; } = DateTime.Now.Date;

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ToDate { get; set; } = DateTime.Now.Date;

        public int EmployeeId { get; set; } = -1;

        public string Employee { get; set; }

        public string TeamIds { get; set; }

        public string Team { get; set; }

        public int RequestType { get; set; } = (int)RequestFilter.All;

        [Display(Name = "Section")]
        public int? LeaveType { get; set; }

        public IList<LeaveType> LeaveTypeCollection { get; set; }

        [Display(Name = "Section")]
        public IList<Team> TeamCollection { get; set; }

        public byte? IsLocal { get; set; }

        public int CurrentUserId { get; set; }

        public RequestViewType ViewType { get; set; }

        public string StatusIds
        {
            get
            {
                string statusIds;
                switch (StatusId)
                {
                    case -1:
                        // status = -1 --> pending and verified (1,2)
                        statusIds = "1,2";
                        break;

                    case 0:
                        statusIds = string.Empty;
                        break;

                    default:
                        statusIds = StatusId.ToString();
                        break;
                }

                return statusIds;
            }
        }

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

                    default:
                        action = string.Empty;
                        break;
                }

                return action;
            }
        }

        public bool IsAllowChangeStatus => ViewType != RequestViewType.CompanySpace;

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

                var teamIds = TeamIds?.Split(',');

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

        public string BreadcrumbText => EnumerationHelper.GetStringValue(ViewType);

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

                    default:
                        pageTitle = Labels.NA;
                        break;
                }

                return pageTitle;
            }
        }
    }
}