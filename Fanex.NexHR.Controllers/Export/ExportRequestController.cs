namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Core;
    using Core.Enums;
    using Criteria.Request;
    using Criteria.Team;
    using Fanex.NexHR.Core.Constants;
    using Models;
    using Models.Request;
    using NPOI.HSSF.UserModel;
    using Resources;
    using Services;
    using Services.ExportExcel;
    using ViewModels.Request;
    using Web.Providers;

    public class ExportRequestController : BaseExportController
    {
        private readonly IExportExcelService exportExcelService;
        private readonly IRequestService requestService;
        private readonly ITeamService teamService;

        public ExportRequestController(
            IExportExcelService exportExcelService,
            IRequestService requestService,
            ITeamService teamService)
        {
            this.exportExcelService = exportExcelService;
            this.requestService = requestService;
            this.teamService = teamService;
        }

        [HRAuthorize(Policy.Host, FunctionKey.VerifyRequest, FunctionKey.ApproveRequest)]
        public ActionResult MemberWorkingTimeRequest(RequestViewModel model)
        {
            model.ViewType = RequestViewType.PersonalSpace;
            return WorkingTimeRequest(model);
        }

        [HRAuthorize(Policy.Host, FunctionKey.ProcessRequest)]
        public ActionResult HRViewWorkingTimeRequest(RequestViewModel model)
        {
            model.ToDate = Utilities.AddTimeToDate(model.ToDate, 23, 59, 59);
            model.ViewType = RequestViewType.HRSpace;

            var criteria = new GetRequestCriteria
            {
                SupervisorId = CurrentUser.Id,
                EmpId = model.EmployeeId == -1 ? null : new int?(model.EmployeeId),
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                GroupIds = model.TeamIds,
                Status = model.StatusIds,
                RequestType = model.RequestType,
                IsQueryByCreatedDate = !model.IsFilterByAffectedDate,
                RequestViewType = model.ViewType,
                IsLocal = model.IsLocal.HasValue ? (model.IsLocal.Value == 1) : (bool?)null
            };

            IList<RequestEntity> requestList = requestService.GetRequests(criteria);
            HSSFWorkbook hssfworkbook = exportExcelService.ExportWorkingTimeRequest(requestList, model.PageTitle);
            string fileName = "EmployeeRequest_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";

            return ExcelFile(hssfworkbook, fileName);
        }

        public ActionResult VoidRequestWorkingTimeRequest(RequestViewModel request)
        {
            request.TeamCollection = GetGroupBySupervisor();
            request.ToDate = Utilities.AddTimeToDate(request.ToDate, 23, 59, 59);
            int? employeeId = null;
            if (request.EmployeeId != -1 && !string.IsNullOrWhiteSpace(request.Employee))
            {
                employeeId = request.EmployeeId;
            }

            bool? isLocal = null;
            var criteria = new GetRequestCriteria
            {
                EmpId = employeeId,
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                GroupIds = request.TeamIds,
                Status = request.StatusIds,
                RequestType = request.RequestType,
                IsQueryByCreatedDate = !request.IsFilterByAffectedDate,
                RequestViewType = request.ViewType,
                SupervisorId = CurrentUser.Id,
                IsLocal = request.IsLocal.HasValue ? (request.IsLocal.Value == 1) : isLocal
            };

            IList<RequestEntity> requestList = requestService.GetRequests(criteria);
            HSSFWorkbook hssfworkbook = exportExcelService.ExportWorkingTimeRequest(requestList, Labels.VoidRequest);
            string fileName = "EmployeeVoidRequest_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";

            return ExcelFile(hssfworkbook, fileName);
        }

        private IList<Team> GetGroupBySupervisor()
        {
            var criteria = new GetGroupBySupervisorCriteria
            {
                SupervisorId = CurrentUser.Id
            };

            IList<Team> teams = teamService.GetBySupervisor(criteria);
            return teams;
        }

        public ActionResult DailyViewWorkingTimeRequest(RequestViewModel model)
        {
            model.ViewType = RequestViewType.CompanySpace;
            return WorkingTimeRequest(model);
        }

        private ActionResult WorkingTimeRequest(RequestViewModel model)
        {
            model.ToDate = Utilities.AddTimeToDate(model.ToDate, 23, 59, 59);
            model.ViewType = RequestViewType.CompanySpace;
            var criteria = new GetRequestCriteria
            {
                EmpId = model.EmployeeId == -1 ? null : new int?(model.EmployeeId),
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                GroupIds = model.TeamIds,
                Status = model.StatusIds,
                RequestType = model.RequestType,
                IsQueryByCreatedDate = !model.IsFilterByAffectedDate,
                RequestViewType = model.ViewType,
                SupervisorId = CurrentUser.Id,
                IsLocal = model.IsLocal.HasValue ? (model.IsLocal.Value == 1) : (bool?)null
            };

            IList<RequestEntity> requestList = requestService.GetRequests(criteria);
            HSSFWorkbook hssfworkbook = exportExcelService.ExportWorkingTimeRequest(requestList, model.PageTitle);
            string fileName = "DailyRequest_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";

            return ExcelFile(hssfworkbook, fileName);
        }
    }
}