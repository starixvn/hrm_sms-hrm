namespace Fanex.NexHR.Controllers
{
    using Core;
    using Core.Enums;
    using Criteria.Request;
    using Models.Request;
    using Nexus.RoleManagement.Services;
    using NPOI.HSSF.UserModel;
    using Services;
    using Services.ExportExcel;
    using Services.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using ViewModels.Request;
    using Web.Providers;

    public class ExportRequestController : BaseExportController
    {
        private readonly IExportExcelService exportExcelService;
        private readonly IRequestService requestService;

        public ExportRequestController(): this(null, null)
        {
        }

        public ExportRequestController(IExportExcelService exportExcelService, IRequestService requestService)
        {
            this.exportExcelService = exportExcelService ?? new ExportExcelService();
            this.requestService = requestService ?? new RequestService();
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