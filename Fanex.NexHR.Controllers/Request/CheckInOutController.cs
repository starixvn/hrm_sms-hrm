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
    using Fanex.NexHR.Criteria.Request.CheckInOut;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels.Request;

    public class CheckInOutController : BaseController
    {
        private readonly ICheckInOutRequestService checkInOutRequestService;
        private readonly IRequestService requestService;
        private readonly IBaseService baseService;
        private readonly IShiftService shiftService;
        const string pathFolder = "~/file/uploads/request/";

        public CheckInOutController(
            ISessionService sessionService,
            ICheckInOutRequestService checkInOutRequestService,
            IRequestService requestService,
            IBaseService baseService,
            IShiftService shiftService) : base(sessionService)
        {
            this.checkInOutRequestService = checkInOutRequestService;
            this.requestService = requestService;
            this.baseService = baseService;
            this.shiftService = shiftService;
        }

        [HttpGet]
        public ActionResult EmployeeView(int? year)
        {
            int tempYear = year ?? DateTime.Today.Year;
            ViewBag.Year = tempYear;
            var criteria = new GetCheckInOutRequestByDateCriteria
            {
                EmpId = sessionService.Employee.Id,
                FromDate = new DateTime(tempYear, 1, 1),
                ToDate = new DateTime(tempYear, 12, 31)
            };

            IList<CheckInOutRequest> requests = checkInOutRequestService.GetByDate(criteria);
            GetInfoAttachFile(requests);

            return View(requests);
        }

        private void GetInfoAttachFile(IEnumerable<CheckInOutRequest> requests)
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
                        content.Append($"<a href=\'{file}\'> {Path.GetFileName(file)}</a></br>");
                    }

                    request.ContentFile = content.ToString();
                }
            }
        }

        public ActionResult Create()
        {
            var today = DateTime.Today;
            VerifierAndApproverForRequest supervisor = requestService.GetVerifierAndApprover(sessionService.Employee.Id, RequestType.CheckIn);
            var viewModel = new CheckInOutRequestViewModel
            {
                Employee = sessionService.Employee.DisplayName,
                EmployeeId = sessionService.Employee.Id,
                ForDate = today,
                AttendantDate = today,
                Hour = DateTime.Now.Hour,
                Minute = DateTime.Now.Minute,
                RequestTypeId = (byte)RequestType.CheckIn,
                Verifiers = supervisor.Verifiers,
                Approvers = supervisor.Approvers
            };

            Shift shift = shiftService.GetByDate(sessionService.Employee.Id, today);
            CheckShiftFromToFormat(viewModel, shift);
            viewModel.Shift = shift == null ? string.Empty : shift.Name;
            ViewBag.IsTopManager = sessionService.Employee.IsTopManager;
            ViewBag.HasOnBehalfRequestPermission = sessionService.HasPermissionOnFunction(FunctionKey.CanOnBehalfOthers, Policy.Read)
                || sessionService.HasPermissionOnFunction(FunctionKey.CanOnBehalfOthers, Policy.Host);

            return View(viewModel);
        }

        [HttpPost]
        public CustomJsonResult Create(string requestModel, IList<HttpPostedFileBase> files)
        {
            CheckInOutRequestViewModel model = JsonConverter.FromJson<CheckInOutRequestViewModel>(requestModel);
            var criteria = new CreateCheckInOutRequestCriteria
            {
                EmpId = model.EmployeeId,
                OnDate = model.AttendantDate,
                RequestTime = model.ForDate + new TimeSpan(model.Hour, model.Minute, 0),
                RequestType = model.RequestTypeId,
                ReasonId = model.ReasonId,
                RefNo = model.RefNO,
                Remark = model.Remark,
                CreatedBy = sessionService.Employee.Id,
                VerifierId = model.VerifierId,
                ApproverId = model.ApproverId
            };
            CommandResult commandResult = baseService.ExecuteNonQuery(criteria);

            if (commandResult.IsSuccessful)
            {
                checkInOutRequestService.SendMailCreateRequest(commandResult.OutputId);
                if (files != null)
                {
                    UploadFile(files, commandResult.OutputId);
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
            CheckInOutRequest request = checkInOutRequestService.GetById(id);
            if (request == null
                || !Utilities.CheckUserCanEditRequest(sessionService.Employee.Id, request.Requester.EmpId)
                || request.Status.Id != (byte)CurrentRequestStatus.Pending)
            {
                return RedirectToAction("AccessDenied", "Error");
            }
            else
            {
                VerifierAndApproverForRequest supervisor = requestService.GetVerifierAndApprover(request.Requester.EmpId, RequestType.CheckIn);
                var param = new GetDisabledDateParam
                {
                    EmployeeId = request.Requester.EmpId,
                    FromDate = request.OnDate.AddMonths(-1),
                    ToDate = DateTime.Today,
                    RequestId = request.RequestId,
                    RequestTypeId = request.RequestTypeId
                };
                IEnumerable<DateTime> disabledDates = checkInOutRequestService.GetDisabledDate(param);
                Shift shift = shiftService.GetByDate(sessionService.Employee.Id, request.OnDate);
                var viewModel = new CheckInOutRequestViewModel
                {
                    RequestId = request.RequestId,
                    RefNO = request.RefNO,
                    RequestTypeId = request.RequestTypeId,
                    Employee = request.Requester.DisplayName,
                    EmployeeId = request.Requester.EmpId,
                    AttendantDate = request.OnDate,
                    ForDate = request.RequestTime.Date,
                    Hour = request.RequestTime.Hour,
                    Minute = request.RequestTime.Minute,
                    ReasonId = request.ReasonId,
                    Shift = shift == null ? string.Empty : shift.Name,
                    ApproverId = request.Approver?.EmpId,
                    VerifierId = request.Verifier?.EmpId,
                    Remark = request.Remark,
                    Verifiers = supervisor.Verifiers,
                    Approvers = supervisor.Approvers,
                    DisabledDates = disabledDates
                };

                if (Directory.Exists(dir))
                {
                    viewModel.Files = GetFileNames(dir);
                }

                ViewBag.IsTopManager = request.Requester.IsTopManager;
                return View(viewModel);
            }
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
            CheckInOutRequestViewModel model = JsonConverter.FromJson<CheckInOutRequestViewModel>(requestModel);
            var criteria = new EditCheckInOutRequestCriteria
            {
                RequestId = model.RequestId,
                EmpId = model.EmployeeId,
                OnDate = model.AttendantDate,
                RequestTime = model.ForDate + new TimeSpan(model.Hour, model.Minute, 0),
                RequestType = model.RequestTypeId,
                ReasonId = model.ReasonId,
                RefNo = model.RefNO,
                Remark = model.Remark,
                ModifiedBy = sessionService.Employee.Id,
                VerifierId = model.VerifierId,
                ApproverId = model.ApproverId
            };

            CommandResult commandResult = baseService.ExecuteNonQuery(criteria);
            if (commandResult.IsSuccessful)
            {
                checkInOutRequestService.SendMailEditRequest(model.RequestId);
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
            checkInOutRequestService.SendMailDeleteRequest(id);
            CommandResult commandResult = checkInOutRequestService.Delete(id);

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
            IList<CheckInOutRequest> requests = checkInOutRequestService.GetHistoryById(id);
            return PartialView("_ViewHistory", requests);
        }

        public CustomJsonResult GetDisabledDate(GetDisabledDateParam param)
        {
            IEnumerable<DateTime> disabledDates = checkInOutRequestService.GetDisabledDate(param);
            return new CustomJsonResult { Data = disabledDates, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        private static void CheckShiftFromToFormat(CheckInOutRequestViewModel viewModel, Shift shift)
        {
            if (shift != null)
            {
                var checkContainColon = shift.BeginCheckInTime.Contains(":");
                if (checkContainColon)
                {
                    var time = shift.BeginCheckInTime.Split(new[] { ':' });
                    viewModel.Hour = int.Parse(time[0]);
                    viewModel.Minute = int.Parse(time[1]);
                }
                else
                {
                    viewModel.Hour = 0;
                    viewModel.Minute = 0;
                }
            }
        }
    }
}