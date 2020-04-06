namespace Fanex.NexHR.Controllers.Request
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
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

    public class KeepInViewController : BaseController
    {
        private readonly IKeepInViewRequestService keepInViewRequestService;
        private readonly IRequestService requestService;
        private readonly IBaseService baseService;
        const string pathFolder = "~/file/uploads/request/";

        public KeepInViewController(
            ISessionService sessionService,
            IKeepInViewRequestService keepInViewRequestService,
            IRequestService requestService,
            IBaseService baseService) : base(sessionService)
        {
            this.keepInViewRequestService = keepInViewRequestService;
            this.requestService = requestService;
            this.baseService = baseService;
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
            GetInfoAttachFile(requests);

            return View(requests);
        }

        private void GetInfoAttachFile(IList<KeepInViewRequest> requests)
        {
            foreach (var request in requests)
            {
                StringBuilder content = new StringBuilder();
                string path = pathFolder + request.RequestId;
                string dir = Server.MapPath(path);
                if (Directory.Exists(dir))
                {
                    string[] files = Directory.GetFiles(dir);

                    foreach (string file in files)
                    {
                        content.Append($"<a href=\'{"/file/uploads/request/" + request.RequestId}/{Path.GetFileName(file)}'> {Path.GetFileName(file)}</a></br>");
                    }

                    request.ContentFile = content.ToString();
                }
            }
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
            ViewBag.HasOnBehalfRequestPermission = sessionService.HasPermissionOnFunction(FunctionKey.CanOnBehalfOthers, Policy.Read)
                || sessionService.HasPermissionOnFunction(FunctionKey.CanOnBehalfOthers, Policy.Host);

            return View(model);
        }

        [HttpPost]
        public CustomJsonResult Create(string requestModel, IList<HttpPostedFileBase> files)
        {
            KeepInViewRequestViewModel model = JsonConverter.FromJson<KeepInViewRequestViewModel>(requestModel);
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
                if (files != null)
                {
                    UploadFile(files, criteria.RequestId);
                }
            }

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        private void UploadFile(IList<HttpPostedFileBase> files, long requestId)
        {
            if (!files.Any(file => file.ContentLength <= 0))
            {
                string dir = Server.MapPath(pathFolder + requestId);

                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                foreach (var file in files)
                {
                    var fileName = Path.GetFileName(file.FileName);

                    var path = Path.Combine(dir, fileName);
                    file.SaveAs(path);
                }
            }
        }

        public ActionResult Edit(long id)
        {
            string dir = Server.MapPath(pathFolder + id);
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
                MaxDate = DateTime.Today.FirstDayOfMonth().AddMonths(2).AddDays(-1)
            };
            viewModel.DisabledDates = keepInViewRequestService.GetDisabledDate(new GetDisabledDateParam
            {
                EmployeeId = request.Requester.EmpId,
                FromDate = viewModel.MinDate,
                ToDate = viewModel.MaxDate,
                RequestId = request.RequestId
            });

            if (Directory.Exists(dir))
            {
                viewModel.Files = GetFileNames(dir);
            }
            ViewBag.IsTopManager = request.Requester.IsTopManager;

            return View(viewModel);
        }

        private static List<string> GetFileNames(string dir)
        {
            List<string> Files = new List<string>();
            string[] files = Directory.GetFiles(dir);
            foreach (string file in files)
            {
                Files.Add(Path.GetFileName(file));
            }

            return Files;
        }

        [HttpPost]
        public CustomJsonResult Edit(string requestModel, IList<HttpPostedFileBase> files, string filesRemove)
        {
            KeepInViewRequestViewModel model = JsonConverter.FromJson<KeepInViewRequestViewModel>(requestModel);
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
                if (files != null){
                    UploadFile(files, criteria.RequestId);
                }
                if (filesRemove != null) {
                    DeleteFile(filesRemove, criteria.RequestId);
                }
            }

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        private void DeleteFile(string filesRemove, long requestId)
        {
            List<string> files = JsonConverter.FromJsonArray<string>(filesRemove);
            foreach (string file in files)
            {
                string dir = Server.MapPath(pathFolder + requestId + '/' + file);
                System.IO.File.Delete(dir);
            }
        }

        [HttpPost]
        public CustomJsonResult Delete(int id)
        {
            keepInViewRequestService.SendMailDeleteRequest(id);
            CommandResult commandResult = keepInViewRequestService.Delete(id);

            if (commandResult.IsSuccessful)
            {
                string dir = Server.MapPath(pathFolder + id);
                if (Directory.Exists(dir))
                {
                    Directory.Delete(dir, true);
                }
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