namespace Fanex.NexHR.Controllers.Request
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Core.Models;
    using Core.Services;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Request;
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
        private readonly IEmployeeService employeeService;

        public CheckInOutController(
            ISessionService sessionService,
            ICheckInOutRequestService checkInOutRequestService,
            IRequestService requestService,
            IBaseService baseService,
            IEmployeeService employeeService,
            IShiftService shiftService) : base(sessionService)
        {
            this.checkInOutRequestService = checkInOutRequestService;
            this.requestService = requestService;
            this.baseService = baseService;
            this.employeeService = employeeService;
            this.shiftService = shiftService;
        }

        [HttpGet]
        public ActionResult EmployeeView()
        {
            return EmployeeView(DateTime.Now.Year);
        }

        [HttpPost]
        public ActionResult EmployeeView(int year)
        {
            ViewBag.Year = year;
            var criteria = new GetRequestCriteria
            {
                EmpId = sessionService.Employee.Id,
                FromDate = new DateTime(year, 1, 1),
                ToDate = new DateTime(year, 12, 31),
                RequestType = (int)RequestFilter.CheckInOut,
                IsQueryByCreatedDate = false,
                RequestViewType = RequestViewType.CompanySpace,
                SupervisorId = sessionService.Employee.Id,
                IsLocal = null
            };

            IList<RequestEntity> entityList = requestService.GetRequests(criteria);
            return View(entityList);
        }

        public ActionResult Create(DateTime? suggestedDate, bool? isMissingCheckIn)
        {
            var viewModel = new CheckInOutRequestViewModel
            {
                Employee = sessionService.Employee.Nickname,
                EmployeeId = sessionService.Employee.Id,
                ForDate = suggestedDate ?? DateTime.Today,
                AttendantDate = suggestedDate ?? DateTime.Today,
                Hour = DateTime.Now.Hour,
                Minute = DateTime.Now.Minute,
                RequestType = (isMissingCheckIn.HasValue && isMissingCheckIn.Value) ? (short)RequestType.CheckIn : (short)RequestType.CheckOut,
                VerifierAndApprover = GetVerifierApprover(sessionService.Employee.Id),
                TotalRequestInMonth = GetTotalRequestInMonth(sessionService.Employee.Id)
            };

            Shift shift = GetShiftByDate(DateTime.Now);
            CheckShiftFromToFormat(viewModel, shift);
            viewModel.Shift = (shift == null) ? string.Empty : shift.Name;

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Create(CheckInOutRequestViewModel model)
        {
            if (model != null)
            {
                CheckIsTopManager(model);
                model.VerifierAndApprover = GetVerifierApprover(model.EmployeeId);
                model.TotalRequestInMonth = GetTotalRequestInMonth(model.EmployeeId);

                if (ModelState.IsValid)
                {
                    var criteria = new InsertCheckInOutRequestCriteria
                    {
                        EmpId = model.EmployeeId,
                        OnDate = model.AttendantDate,
                        CheckTime = model.ForDate + new TimeSpan(model.Hour, model.Minute, 0),
                        RequestType = model.RequestType,
                        RefNo = model.RefNO,
                        Remark = model.Remark,
                        CreatedBy = sessionService.Employee.Id,
                        VerifierId = model.VerifierId,
                        ApproverId = model.ApproverId,
                        Shift = model.Shift
                    };
                    CommandResult result = baseService.ExecuteNonQuery(criteria, hasGetOutputParams: true);

                    if (result.IsSuccessful)
                    {
                        checkInOutRequestService.SendMailForInsertSuccessfully(criteria);
                        ViewBag.Requested = true;
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, result.Message);
                    }
                }
            }

            return View(model);
        }

        public ActionResult Update(int id)
        {
            var criteria = new GetRequestByIdCriteria
            {
                RequestId = id
            };
            RequestEntity request = requestService.GetRequestById(criteria);
            if (request == null || !Utilities.CheckUserCanEditRequest(sessionService.Employee.Id, request.Requester.Id))
            {
                return RedirectToAction("AccessDenied", "Error");
            }
            else
            {
                Shift shift = GetShiftByDate(request.RequestedDate);
                var viewModel = new CheckInOutRequestViewModel
                {
                    RequestId = request.Id,
                    RequestType = request.RequestType.Id,
                    Employee = sessionService.Employee.Nickname,
                    EmployeeId = sessionService.Employee.Id,
                    ForDate = request.RequestedDate.Date,
                    Hour = request.RequestedDate.Hour,
                    Minute = request.RequestedDate.Minute,
                    Shift = (shift == null) ? string.Empty : shift.Name,
                    ApproverId = request.ApprovedBy.Id,
                    VerifierId = request.VerifiedBy.Id,
                    RefNO = request.RefNO,
                    AttendantDate = request.FromDate,
                    Remark = request.Remark,
                    VerifierAndApprover = GetVerifierApprover(sessionService.Employee.Id)
                };

                return View(viewModel);
            }
        }

        [HttpPost]
        public ActionResult Update(CheckInOutRequestViewModel model)
        {
            if (model != null)
            {
                CheckIsTopManager(model);
                model.VerifierAndApprover = GetVerifierApprover(model.EmployeeId);

                if (ModelState.IsValid)
                {
                    var criteria = new UpdateCheckInOutRequestCriteria
                    {
                        RequestId = model.RequestId,
                        EmpId = model.EmployeeId,
                        OnDate = model.AttendantDate,
                        CheckTime = model.ForDate + new TimeSpan(model.Hour, model.Minute, 0),
                        RequestType = model.RequestType,
                        RefNo = model.RefNO,
                        Remark = model.Remark,
                        ModifiedBy = sessionService.Employee.Id,
                        VerifierId = model.VerifierId,
                        ApproverId = model.ApproverId,
                        Shift = model.Shift
                    };

                    CommandResult result = baseService.ExecuteNonQuery(criteria);
                    if (result.IsSuccessful)
                    {
                        checkInOutRequestService.SendMailForUpdateSuccessfully(criteria);
                        ViewBag.Updated = true;
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, result.Message);
                    }
                }
            }

            return View(model);
        }

        private decimal GetTotalRequestInMonth(int employeeId)
        {
            var currentDate = DateTime.Now.Date;
            var criteria = new GetNumberRequestInMonthCriteria
            {
                EmployeeId = employeeId,
                RequestType = (int)RequestType.CheckIn,
                FromDate = new DateTime(currentDate.Year, currentDate.Month, 1),
                ToDate = new DateTime(currentDate.Year, currentDate.Month, DateTime.DaysInMonth(currentDate.Year, currentDate.Month))
            };
            decimal numberRequests = requestService.GetNumberRequestInMonth(criteria);
            return numberRequests;
        }

        private VerifierAndApproverForRequest GetVerifierApprover(int employeeId)
        {
            var criteria = new GetVerifierAndApproverCriteria
            {
                EmpId = employeeId,
                RequestType = (byte)RequestType.CheckIn
            };
            VerifierAndApproverForRequest supervisor = requestService.GetVerifierAndApprover(criteria);
            return supervisor;
        }

        private Shift GetShiftByDate(DateTime date)
        {
            Shift shift = shiftService.GetByDate(sessionService.Employee.Id, date);

            return shift;
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

        private void CheckIsTopManager(CheckInOutRequestViewModel model)
        {
            Employee employee = employeeService.GetEmployeeById(model.EmployeeId);
            if (employee.IsTopManager)
            {
                foreach (var modelError in ModelState)
                {
                    string propertyName = modelError.Key;

                    if (propertyName.Contains("VerifierId") || propertyName.Contains("ApproverId"))
                    {
                        ModelState[propertyName].Errors.Clear();
                    }
                }
            }
        }
    }
}