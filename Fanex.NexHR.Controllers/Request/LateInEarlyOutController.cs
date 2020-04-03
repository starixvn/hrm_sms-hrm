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
    using Fanex.NexHR.Criteria.Request.LateInEarlyOut;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels.Request;

    public class LateInEarlyOutController : BaseController
    {
        private const int DefaultMinutes = 60;
        private readonly ILateInEarlyOutRequestService lateInEarlyOutRequestService;
        private readonly IRequestService requestService;
        private readonly IBaseService baseService;
        const string pathFolder = "~/file/uploads/request/";

        public LateInEarlyOutController(
            ISessionService sessionService,
            ILateInEarlyOutRequestService lateInEarlyOutRequestService,
            IRequestService requestService,
            IBaseService baseService) : base(sessionService)
        {
            this.lateInEarlyOutRequestService = lateInEarlyOutRequestService;
            this.requestService = requestService;
            this.baseService = baseService;
        }

        [HttpGet]
        public ActionResult EmployeeView(int? year)
        {
            int tempYear = year ?? DateTime.Today.Year;
            ViewBag.Year = tempYear;
            var criteria = new GetLateInEarlyOutRequestByDateCriteria
            {
                EmpId = sessionService.Employee.Id,
                FromDate = new DateTime(tempYear, 1, 1),
                ToDate = new DateTime(tempYear, 12, 31)
            };
            IList<LateInEarlyOutRequest> requests = lateInEarlyOutRequestService.GetByDate(criteria);
            GetInfoAttachFile(requests);
            return View(requests);
        }

        private void GetInfoAttachFile(IList<LateInEarlyOutRequest> requests)
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
            VerifierAndApproverForRequest supervisor = requestService.GetVerifierAndApprover(sessionService.Employee.Id, RequestType.LateIn);
            var viewModel = new LateInEarlyOutRequestViewModel
            {
                Employee = sessionService.Employee.DisplayName,
                EmployeeId = sessionService.Employee.Id,
                OnDate = DateTime.Today,
                Minute = DefaultMinutes,
                ReasonId = (byte)LateInEarlyOutReason.Other,
                RequestType = (byte)RequestType.LateIn,
                Verifiers = supervisor.Verifiers,
                Approvers = supervisor.Approvers
            };
            ViewBag.IsTopManager = sessionService.Employee.IsTopManager;
            ViewBag.HasOnBehalfRequestPermission = sessionService.HasPermissionOnFunction(FunctionKey.CanOnBehalfOthers, Policy.Read)
                || sessionService.HasPermissionOnFunction(FunctionKey.CanOnBehalfOthers, Policy.Host);

            return View(viewModel);
        }

        [HttpPost]
        public CustomJsonResult Create(string requestModel, IList<HttpPostedFileBase> files)
        {
            LateInEarlyOutRequestViewModel model = JsonConverter.FromJson<LateInEarlyOutRequestViewModel>(requestModel);

            var criteria = new InsertLateInEarlyOutRequestCriteria
            {
                EmpId = model.EmployeeId,
                OnDate = model.OnDate,
                RequestType = model.RequestType,
                Remark = model.Remark,
                LateInMins = model.RequestType == (byte)RequestType.LateIn ? model.Minute : 0,
                EarlyOutMins = model.RequestType == (byte)RequestType.EarlyOut ? model.Minute : 0,
                ReasonId = model.ReasonId,
                VerifierId = model.VerifierId,
                ApproverId = model.ApproverId,
                CreatedBy = sessionService.Employee.Id
            };
            CommandResult commandResult = baseService.ExecuteNonQuery(criteria);
            if (commandResult.IsSuccessful)
            {
                lateInEarlyOutRequestService.SendMailCreateRequest(commandResult.OutputId);
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


        public ActionResult Edit(int id)
        {
            string dir = Server.MapPath(pathFolder + id);
            LateInEarlyOutRequest request = lateInEarlyOutRequestService.GetById(id);
            if (request == null
                || !Utilities.CheckUserCanEditRequest(sessionService.Employee.Id, request.Requester.EmpId)
                || request.Status.Id != (byte)CurrentRequestStatus.Pending)
            {
                return RedirectToAction("AccessDenied", "Error");
            }

            VerifierAndApproverForRequest supervisor = requestService.GetVerifierAndApprover(request.Requester.EmpId, RequestType.LateIn);
            var minutes = request.RequestTypeId == (byte)RequestType.LateIn ? request.LateInMins : request.EarlyOutMins;
            var param = new GetDisabledDateParam
            {
                EmployeeId = request.Requester.EmpId,
                FromDate = request.OnDate.AddMonths(-1),
                ToDate = DateTime.Today,
                RequestId = request.RequestId,
                RequestTypeId = request.RequestTypeId,
            };
            IEnumerable<DateTime> disabledDates = lateInEarlyOutRequestService.GetDisabledDate(param);
            var viewModel = new LateInEarlyOutRequestViewModel
            {
                RequestId = request.RequestId,
                Employee = request.Requester.DisplayName,
                EmployeeId = request.Requester.EmpId,
                OnDate = request.OnDate,
                Minute = minutes,
                RequestType = request.RequestTypeId,
                Remark = request.Remark,
                ReasonId = request.ReasonId,
                ApproverId = request.Approver?.EmpId,
                VerifierId = request.Verifier?.EmpId,
                DisabledDates = disabledDates,
                Verifiers = supervisor.Verifiers,
                Approvers = supervisor.Approvers
            };

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
        public CustomJsonResult Edit(string requestModel, IList<HttpPostedFileBase> files)
        {
            LateInEarlyOutRequestViewModel model = JsonConverter.FromJson<LateInEarlyOutRequestViewModel>(requestModel);

            var criteria = new UpdateLateInEarlyOutRequestCriteria
            {
                RequestId = model.RequestId,
                EmpId = model.EmployeeId,
                OnDate = model.OnDate,
                RequestType = model.RequestType,
                Remark = model.Remark,
                ReasonId = model.ReasonId,
                LateInMins = model.RequestType == (short)RequestType.LateIn ? model.Minute : 0,
                EarlyOutMins = model.RequestType == (short)RequestType.EarlyOut ? model.Minute : 0,
                VerifierId = model.VerifierId,
                ApproverId = model.ApproverId,
                ModifiedBy = sessionService.Employee.Id
            };

            CommandResult commandResult = baseService.ExecuteNonQuery(criteria);
            if (commandResult.IsSuccessful)
            {
                lateInEarlyOutRequestService.SendMailEditRequest(model.RequestId);
                if (files != null)
                {
                    UploadFile(files, criteria.RequestId);
                }
            }

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        public CustomJsonResult Delete(int id)
        {
            lateInEarlyOutRequestService.SendMailDeleteRequest(id);
            CommandResult commandResult = lateInEarlyOutRequestService.Delete(id);

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
            var requests = lateInEarlyOutRequestService.GetHistoryById(id);
            return PartialView("_ViewHistory", requests);
        }

        public CustomJsonResult GetActualLateInEarlyOut(int employeeId, DateTime workDate)
        {
            var criteria = new GetActualLateInEarlyOutCriteria
            {
                EmployeeId = employeeId,
                WorkDate = workDate
            };

            EmployeeLateInEarlyOutModel lateInEarlyOut = lateInEarlyOutRequestService.GetActualLateInEarlyOut(criteria);
            lateInEarlyOut = lateInEarlyOut ?? new EmployeeLateInEarlyOutModel();

            return new CustomJsonResult { Data = new { lateInEarlyOut.LateIn, lateInEarlyOut.EarlyOut }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public CustomJsonResult GetDisabledDate(GetDisabledDateParam param)
        {
            IEnumerable<DateTime> disabledDates = lateInEarlyOutRequestService.GetDisabledDate(param);
            return new CustomJsonResult { Data = disabledDates, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
    }
}