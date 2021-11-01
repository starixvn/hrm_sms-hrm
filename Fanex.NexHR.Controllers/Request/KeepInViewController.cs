namespace Fanex.NexHR.Controllers.Request
{
    using System;
    using System.Collections.Generic;
    using System.Web;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Criteria.Request.KeepInView;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels.Request;
    using HRM.UserControl.Client.Core.Enumerables;
    using HRM.UserControl.Client.Core.Session;

    public class KeepInViewController : BaseController
    {
        private const string UploadFolderPath = "/file/uploads/request/";
        private readonly IUserControlSession userControlSession;
        private readonly IKeepInViewRequestService keepInViewRequestService;
        private readonly IRequestService requestService;
        private readonly IBaseService baseService;
        private readonly IAttachFileService attachFileService;

        public KeepInViewController(
            ISessionService sessionService,
            IUserControlSession userControlSession,
            IKeepInViewRequestService keepInViewRequestService,
            IRequestService requestService,
            IBaseService baseService,
            IAttachFileService attachFileService) : base(sessionService)
        {
            this.userControlSession = userControlSession;
            this.keepInViewRequestService = keepInViewRequestService;
            this.requestService = requestService;
            this.baseService = baseService;
            this.attachFileService = attachFileService;
        }

        [HttpGet]
        public ActionResult EmployeeView(int? year)
        {
            int tempYear = year ?? DateTime.Today.Year;
            ViewBag.Year = tempYear;
            var criteria = new GetKeepInViewRequestByDateCriteria
            {
                EmpId = sessionService.Employee.Id,
                FromDate = new DateTime(tempYear, 1, 1),
                ToDate = new DateTime(tempYear, 12, 31)
            };
            IList<KeepInViewRequest> requests = keepInViewRequestService.GetByDate(criteria);
            foreach (var request in requests)
            {
                request.AttachFiles = attachFileService.GetInfoAttachFile(request.RequestId, UploadFolderPath);
            }

            return View(requests);
        }

        public ActionResult Create()
        {
            var today = DateTime.Today;
            VerifierAndApproverForRequest supervisor = requestService.GetVerifierAndApprover(sessionService.Employee.Id, RequestType.KeepInView);
            var model = new KeepInViewRequestViewModel
            {
                Employee = sessionService.Employee.DisplayName,
                EmployeeId = sessionService.Employee.Id,
                Date = today,
                KeepInViewType = (byte)KeepInViewTypes.KeepInViewByDay,
                Verifiers = supervisor.Verifiers,
                Approvers = supervisor.Approvers,
                MinDate = today.FirstDayOfMonth().AddMonths(-1),
                MaxDate = today.FirstDayOfMonth().AddMonths(2).AddDays(-1)
            };
            ViewBag.IsTopManager = sessionService.Employee.IsTopManager;
            ViewBag.HasOnBehalfRequestPermission = userControlSession.HasPermissionOnFunction(FunctionKey.CanOnBehalfOthers, Policy.Read)
                || userControlSession.HasPermissionOnFunction(FunctionKey.CanOnBehalfOthers, Policy.Host);

            return View(model);
        }

        [HttpPost]
        public CustomJsonResult Create(string requestModel, IList<HttpPostedFileBase> files)
        {
            var model = JsonConverter.FromJson<KeepInViewRequestViewModel>(requestModel);
            var fromTime = DateTime.Parse(model.Date.ToDateMonthYearFormatedDate() + " " + model.FromTime);
            var toTime = DateTime.Parse(model.Date.ToDateMonthYearFormatedDate() + " " + model.ToTime);
            var criteria = new CreateKeepInViewRequestCriteria
            {
                EmpId = model.EmployeeId,
                OnDate = model.Date,
                KeepInViewType = model.KeepInViewType,
                IsHalfDay = model.HalfDay,
                Remark = model.Remark,
                FromTime = (fromTime < toTime && model.CrossDay)
                        ? DateTime.Parse(model.Date.AddDays(1).ToDateMonthYearFormatedDate() + " " + model.FromTime)
                        : DateTime.Parse(model.Date.ToDateMonthYearFormatedDate() + " " + model.FromTime),
                ToTime = model.CrossDay
                        ? DateTime.Parse(model.Date.AddDays(1).ToDateMonthYearFormatedDate() + " " + model.ToTime)
                        : DateTime.Parse(model.Date.ToDateMonthYearFormatedDate() + " " + model.ToTime),
                VerifierId = model.VerifierId,
                ApproverId = model.ApproverId,
                CreatedBy = sessionService.Employee.Id
            };

            CommandResult commandResult = baseService.ExecuteNonQuery(criteria);
            if (commandResult.IsSuccessful)
            {
                keepInViewRequestService.SendMailCreateRequest(commandResult.OutputId);
                attachFileService.UploadFile(files, criteria.RequestId, UploadFolderPath);
            }

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult Edit(long id)
        {
            KeepInViewRequest request = keepInViewRequestService.GetById(id);
            if (request == null
                || !Utilities.CheckUserCanEditRequest(sessionService.Employee.Id, request.Requester.EmpId)
                || request.Status.Id != (byte)CurrentRequestStatus.Pending)
            {
                return RedirectToAction("AccessDenied", "Error");
            }

            VerifierAndApproverForRequest supervisor = requestService.GetVerifierAndApprover(request.Requester.EmpId, RequestType.KeepInView);
            var viewModel = new KeepInViewRequestViewModel
            {
                RequestId = id,
                RefNo = request.RefNO,
                HalfDay = request.IsHalfDay,
                Employee = request.Requester.DisplayName,
                EmployeeId = request.Requester.EmpId,
                Date = request.OnDate,
                KeepInViewType = request.KeepInViewTypeId,
                ApproverId = request.Approver?.EmpId,
                VerifierId = request.Verifier?.EmpId,
                Remark = request.Remark,
                CrossDay = (request.ToTime - request.OnDate).Days > 0,
                FromTime = request.FromTime.ToString("HH:mm"),
                ToTime = request.ToTime.ToString("HH:mm"),
                Verifiers = supervisor.Verifiers,
                Approvers = supervisor.Approvers,
                MinDate = request.OnDate.AddMonths(-1),
                MaxDate = DateTime.Today.FirstDayOfMonth().AddMonths(2).AddDays(-1),
                Files = attachFileService.GetAttachFile(id, UploadFolderPath)
            };

            ViewBag.IsTopManager = request.Requester.IsTopManager;

            return View(viewModel);
        }

        [HttpPost]
        public CustomJsonResult Edit(string requestModel, IList<HttpPostedFileBase> files, string filesRemove)
        {
            var model = JsonConverter.FromJson<KeepInViewRequestViewModel>(requestModel);
            var criteria = new EditKeepInViewRequestCriteria
            {
                RequestId = model.RequestId,
                EmpId = model.EmployeeId,
                OnDate = model.Date,
                KeepInViewType = model.KeepInViewType,
                Remark = model.Remark,
                IsHalfDay = model.HalfDay,
                FromTime = DateTime.Parse(model.Date.ToDateMonthYearFormatedDate() + " " + model.FromTime),
                ToTime = model.CrossDay
                        ? DateTime.Parse(model.Date.AddDays(1).ToDateMonthYearFormatedDate() + " " + model.ToTime)
                        : DateTime.Parse(model.Date.ToDateMonthYearFormatedDate() + " " + model.ToTime),
                VerifierId = model.VerifierId,
                ApproverId = model.ApproverId,
                ModifiedBy = sessionService.Employee.Id,
            };

            CommandResult commandResult = baseService.ExecuteNonQuery(criteria);
            if (commandResult.IsSuccessful)
            {
                keepInViewRequestService.SendMailEditRequest(model.RequestId);
                attachFileService.UploadFile(files, criteria.RequestId, UploadFolderPath);
                if (!string.IsNullOrWhiteSpace(filesRemove))
                {
                    attachFileService.DeleteFile(JsonConverter.FromJsonArray<string>(filesRemove), criteria.RequestId, UploadFolderPath);
                }
            }

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        public CustomJsonResult Delete(int id)
        {
            keepInViewRequestService.SendMailDeleteRequest(id);
            CommandResult commandResult = keepInViewRequestService.Delete(id);

            if (commandResult.IsSuccessful)
            {
                attachFileService.DeleteFolderAttachFile(id, UploadFolderPath);
            }

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult ViewHistory(long id)
        {
            IList<KeepInViewRequest> requests = keepInViewRequestService.GetHistoryById(id);
            return PartialView("_ViewHistory", requests);
        }

        public CustomJsonResult GetDisabledDate(GetDisabledDateParam param)
        {
            IEnumerable<DateTime> disabledDates = keepInViewRequestService.GetDisabledDate(param);
            return new CustomJsonResult { Data = disabledDates, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
    }
}