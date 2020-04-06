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
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Models.Request.ChangeShift;
    using Fanex.NexHR.Models.ScheduleArrange;
    using Fanex.NexHR.Models.Supervisor;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels.Request;

    public class ChangeShiftController : BaseController
    {
        private readonly IChangeShiftRequestService changeShiftService;
        private readonly IRequestService requestService;
        private readonly IShiftService shiftService;
        const string pathFolder = "~/file/uploads/request/";

        public ChangeShiftController(
            ISessionService sessionService,
            IChangeShiftRequestService changeShiftService,
            IRequestService requestService,
            IShiftService shiftService) : base(sessionService)
        {
            this.changeShiftService = changeShiftService;
            this.requestService = requestService;
            this.shiftService = shiftService;
        }

        [HttpGet]
        public ActionResult EmployeeView(int? year)
        {
            int tempYear = year ?? DateTime.Today.Year;
            var fromDate = new DateTime(tempYear, 1, 1);
            var toDate = new DateTime(tempYear, 12, 31).ToEndOfDay();
            IEnumerable<ChangeShiftRequest> requests = changeShiftService.GetByEmployee(sessionService.Employee.Id, fromDate, toDate);
            GetInfoAttachFile(requests);
            ViewBag.Year = tempYear;
            return View(requests);
        }

        private void GetInfoAttachFile(IEnumerable<ChangeShiftRequest> requests)
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
            VerifierAndApproverForRequest supervisor = GetVerifierApprover(sessionService.Employee.Id);
            var model = new ChangeShiftViewModel
            {
                Employee = sessionService.Employee.DisplayName,
                EmployeeId = sessionService.Employee.Id,
                FromDate = today,
                ToDate = today,
                TotalRequestInMonth = changeShiftService.GetNumberOfRequest(sessionService.Employee.Id, today.FirstDayOfMonth(), today.LastDayOfMonth()),
                Verifiers = supervisor.Verifiers,
                Approvers = supervisor.Approvers
            };
            ViewBag.IsHost = true;
            ViewBag.IsTopManager = sessionService.Employee.IsTopManager;
            ViewBag.HasOnBehalfRequestPermission = sessionService.HasPermissionOnFunction(FunctionKey.CanOnBehalfOthers, Policy.Read)
                || sessionService.HasPermissionOnFunction(FunctionKey.CanOnBehalfOthers, Policy.Host);
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public CustomJsonResult Create(string model, string details, IList<HttpPostedFileBase> files)
        {
            CreateChangeShift criteria = JsonConverter.FromJson<CreateChangeShift>(model);
            List<ChangeShiftDetail> requestDetail = JsonConverter.FromJsonArray<ChangeShiftDetail>(details);
            criteria.Details = requestDetail;

            CommandResult commandResult = changeShiftService.Create(sessionService.Employee.Id, criteria);

            if (commandResult.IsSuccessful)
            {
                changeShiftService.SendMailCreateRequest(commandResult.OutputId);
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

        public ActionResult Edit(int id)
        {
            string dir = Server.MapPath(pathFolder + id);

            ChangeShiftRequest request = changeShiftService.GetById(id);
            if (request == null
                || !Utilities.CheckUserCanEditRequest(sessionService.Employee.Id, request.Requester.EmpId)
                || request.Status.Id != (byte)CurrentRequestStatus.Pending)
            {
                return RedirectToAction("AccessDenied", "Error");
            }

            VerifierAndApproverForRequest supervisor = GetVerifierApprover(request.Requester.EmpId);
            var model = new ChangeShiftViewModel
            {
                RequestId = request.RequestId,
                Employee = request.Requester.DisplayName,
                EmployeeId = request.Requester.EmpId,
                FromDate = request.FromDate.Date,
                ToDate = request.ToDate.Date,
                VerifierId = request.Verifier?.EmpId,
                ApproverId = request.Approver?.EmpId,
                Remark = request.Remark,
                Details = changeShiftService.GetDetailById(id),
                Verifiers = supervisor.Verifiers,
                Approvers = supervisor.Approvers
            };

            if (request.TypeId == (byte)ChangeShiftType.MoveOffDay)
            {
                foreach (ChangeShiftDetail detail in model.Details)
                {
                    detail.DisableDates = changeShiftService.GetDisableDateForMoveOffDayRequest(model.EmployeeId, detail.OnDate, model.RequestId);
                }
            }

            if (Directory.Exists(dir))
            {
                model.Files = GetFileNames(dir);
            }

            ViewBag.IsHost = true;
            ViewBag.IsTopManager = request.Requester.IsTopManager;
            ViewBag.TypeId = request.TypeId;
            ViewBag.Shifts = shiftService.GetAll(isShowOff: true);
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public CustomJsonResult Edit(string model, string details, IList<HttpPostedFileBase> files, string filesRemove)
        {
            EditChangeShift criteria = JsonConverter.FromJson<EditChangeShift>(model);
            List<ChangeShiftDetail> requestDetail = JsonConverter.FromJsonArray<ChangeShiftDetail>(details);
            criteria.Details = requestDetail;

            CommandResult commandResult = changeShiftService.Edit(sessionService.Employee.Id, criteria);
            if (commandResult.IsSuccessful)
            {
                changeShiftService.SendMailEditRequest(criteria.RequestId);
                if (files != null)
                {
                    UploadFile(files, criteria.RequestId);
                }
                if (filesRemove != null)
                {
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

        public CustomJsonResult Delete(long id)
        {
            changeShiftService.SendMailDeleteRequest(id);
            CommandResult commandResult = changeShiftService.Delete(id);

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

        public ActionResult Details(int id)
        {
            string dir = Server.MapPath(pathFolder + id);

            ChangeShiftRequest request = changeShiftService.GetById(id);
            if (request == null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }

            VerifierAndApproverForRequest supervisor = GetVerifierApprover(request.Requester.EmpId);
            var model = new ChangeShiftViewModel
            {
                Employee = request.Requester.DisplayName,
                EmployeeId = request.Requester.EmpId,
                FromDate = request.FromDate.Date,
                ToDate = request.ToDate.Date,
                VerifierId = request.Verifier?.EmpId,
                ApproverId = request.Approver?.EmpId,
                Remark = request.Remark,
                Details = changeShiftService.GetDetailById(id),
                Status = request.Status,
                Verifiers = supervisor.Verifiers,
                Approvers = supervisor.Approvers,
            };

            if (Directory.Exists(dir))
            {
                model.Files = GetFileNames(dir);
            }

            ViewBag.IsHost = false;
            ViewBag.TypeId = request.TypeId;
            ViewBag.Shifts = shiftService.GetAll(isShowOff: true);
            return View(model);
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

        public ActionResult GetChangeShiftDetail(GetWorkingCalendarForRequestParam param)
        {
            IEnumerable<DailySchedule> dailySchedules = changeShiftService.GetWorkingCalendarForRequest(param);
            IList<ChangeShiftDetail> details = dailySchedules.Select(x => new ChangeShiftDetail
            {
                OnDate = x.WorkDate,
                Remark = string.Empty,
                FromShiftId = GetFromShiftId(x),
                FromShift = GetFromShiftName(x)
            }).ToList();

            if (param.TypeId == (byte)ChangeShiftType.MoveOffDay)
            {
                foreach (ChangeShiftDetail detail in details)
                {
                    detail.DisableDates = changeShiftService.GetDisableDateForMoveOffDayRequest(param.EmployeeId, detail.OnDate, param.RequestId);
                }
            }

            ViewBag.IsHost = true;
            ViewBag.Shifts = shiftService.GetAll(isShowOff: true);
            ViewBag.TypeId = param.TypeId;

            return PartialView("_ChangeShiftDetail", details);
        }

        public ActionResult ViewHistory(long id)
        {
            IEnumerable<ChangeShiftRequest> requests = changeShiftService.GetHistoryById(id);
            return PartialView("_ViewHistory", requests);
        }

        private static int GetFromShiftId(DailySchedule dailySchedule)
        {
            if (dailySchedule.IsOffDay)
            {
                return Constants.OffDayShiftId;
            }

            return dailySchedule.Shift != null ? dailySchedule.Shift.Id : 0;
        }

        private static string GetFromShiftName(DailySchedule dailySchedule)
        {
            if (dailySchedule.IsOffDay)
            {
                return Labels.Off;
            }

            return dailySchedule.Shift != null ? dailySchedule.Shift.Name : Labels.NA;
        }

        private VerifierAndApproverForRequest GetVerifierApprover(int employeeId, RequestEntity request = null)
        {
            VerifierAndApproverForRequest supervisor = requestService.GetVerifierAndApprover(employeeId, RequestType.ChangeShift);

            if (request != null)
            {
                if (request.VerifiedBy != null && !supervisor.Verifiers.Any(t => t.OwnerId == request.VerifiedBy.Id))
                {
                    var verifier = new SupervisorNameModel
                    {
                        OwnerId = request.VerifiedBy.Id,
                        NickName = request.VerifiedBy.NickName,
                        FullName = request.VerifiedBy.FullName
                    };
                    supervisor.Verifiers.Add(verifier);
                }

                if (request.ApprovedBy != null && !supervisor.Approvers.Any(t => t.OwnerId == request.ApprovedBy.Id))
                {
                    var approver = new SupervisorNameModel
                    {
                        OwnerId = request.ApprovedBy.Id,
                        NickName = request.ApprovedBy.NickName,
                        FullName = request.ApprovedBy.FullName
                    };
                    supervisor.Approvers.Add(approver);
                }
            }

            return supervisor;
        }
    }
}