namespace Fanex.NexHR.Controllers.Request
{
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Criteria.Request.KeepInView;
    using Fanex.NexHR.Criteria.Shift;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.Request;
    using Nexus.RoleManagement.Services;
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;

    public class KeepInViewController : BaseController
    {
        private readonly IKeepInViewRequestService keepInViewRequestService;
        private readonly IRequestService requestService;
        private readonly ICommandService commandService;
        private readonly IShiftService shiftService;
        private readonly IEmployeeService employeeService;

        public KeepInViewController()
            : this(null, null, null, null, null)
        {
        }

        public KeepInViewController(
            IKeepInViewRequestService keepInViewRequestService, 
            IRequestService requestService, 
            ICommandService commandService,
            IShiftService shiftService,
            IEmployeeService employeeService)
        {
            this.keepInViewRequestService = keepInViewRequestService ?? new KeepInViewRequestService();
            this.requestService = requestService ?? new RequestService();
            this.commandService = commandService ?? new CommandService();
            this.shiftService = shiftService ?? new ShiftService();
            this.employeeService = employeeService ?? new EmployeeService();
        }

        public ActionResult Create()
        {
            var getShiftByDateCriteria = new GetEmployeeShiftByDateCriteria()
            {
                EmployeeId = CurrentUser.Id,
                Date = DateTime.Now
            };
            Shift shift = shiftService.GetByDate(getShiftByDateCriteria);

            var model = new KeepInViewRequestViewModel
            {
                Employee = CurrentUser.NickName,
                EmployeeId = CurrentUser.Id,
                Date = DateTime.Now.Date,
                Shift = (shift == null) ? string.Empty : shift.Name,
                ShiftId = (shift == null) ? 0 : shift.Id,
                UnavailDates = GetUnavailDate(CurrentUser.Id),
                VerifierAndApprover = GetVerifierApprover(CurrentUser.Id)
            };
            ViewBag.HasOnBehalfOthersPermission = CurrentUser.HasPermission(FunctionKey.CanOnBehalfOthers, Policy.Edit);

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(KeepInViewRequestViewModel model)
        {
            if (model != null)
            {
                model.VerifierAndApprover = GetVerifierApprover(model.EmployeeId);
                model.UnavailDates = GetUnavailDate(model.EmployeeId);

                if (ModelState.IsValid)
                {
                    var criteria = new InsertKeepInViewRequestCriteria()
                    {
                        EmpId = model.EmployeeId,
                        OnDate = model.Date,
                        RequestType = (int)RequestType.KeepInView,
                        RefNo = model.RefNo,
                        Remark = model.Remark,
                        IsHalfDay = model.HalfDay,
                        ShiftId = model.ShiftId,
                        ShiftName = model.Shift,
                        FromTime = DateTime.Parse(model.Date.ToDateMonthYearFormatedDate() + " " + model.FromTime),
                        ToTime = model.CrossDay
                        ? DateTime.Parse(model.Date.AddDays(1).ToDateMonthYearFormatedDate() + " " + model.ToTime)
                        : DateTime.Parse(model.Date.ToDateMonthYearFormatedDate() + " " + model.ToTime),
                        VerifierId = model.VerifierId,
                        ApproverId = model.ApproverId,
                        CreatedBy = CurrentUser.Id
                    };

                    CommandResult result = commandService.Insert(criteria, hasGetOutputParams: true);
                    if (result.IsSuccessful)
                    {
                        keepInViewRequestService.SendMailForInsertSuccessfully(criteria);
                        ViewBag.Requested = true;
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, result.Message);
                    }
                }
            }

            ViewBag.HasOnBehalfOthersPermission = CurrentUser.HasPermission(FunctionKey.CanOnBehalfOthers, Policy.Edit);

            return View(model);
        }

        public ActionResult Update(long id)
        {
            var criteria = new GetRequestByIdCriteria()
            {
                RequestId = id
            };
            RequestEntity request = requestService.GetRequestById(criteria);
            if (request == null || !Utilities.CheckUserCanEditRequest(CurrentUser.Id, request.Requester.Id))
            {
                return RedirectToAction("AccessDenied", "Home");
            }
            else
            {
                var viewModel = new KeepInViewRequestViewModel()
                {
                    RequestId = id,
                    RefNo = request.RefNO,
                    HalfDay = request.IsHalfDay,
                    Employee = CurrentUser.NickName,
                    EmployeeId = CurrentUser.Id,
                    Date = request.RequestedDate,
                    Shift = request.FromShift.Name,
                    ShiftId = request.FromShift.Id,
                    ApproverId = request.ApprovedBy.Id,
                    VerifierId = request.VerifiedBy.Id,
                    Remark = request.Remark,
                    CrossDay = request.CrossDay,
                    FromTime = request.FromTime.ToString("HH:mm"),
                    ToTime = request.ToTime.ToString("HH:mm"),
                    VerifierAndApprover = GetVerifierApprover(CurrentUser.Id),
                    UnavailDates = GetUnavailDate(CurrentUser.Id)
                };
                ViewBag.HasOnBehalfOthersPermission = CurrentUser.HasPermission(FunctionKey.CanOnBehalfOthers, Policy.Edit);

                return View(viewModel);
            }
        }

        [HttpPost]
        public ActionResult Update(KeepInViewRequestViewModel model)
        {
            if (model != null)
            {
                model.VerifierAndApprover = GetVerifierApprover(model.EmployeeId);
                model.UnavailDates = GetUnavailDate(model.EmployeeId);

                if (ModelState.IsValid)
                {
                    var criteria = new UpdateKeepInViewRequestCriteria()
                    {
                        RequestId = model.RequestId,
                        EmpId = model.EmployeeId,
                        OnDate = model.Date,
                        RequestType = (int)RequestType.KeepInView,
                        RefNo = model.RefNo,
                        Remark = model.Remark,
                        IsHalfDay = model.HalfDay,
                        ShiftId = model.ShiftId,
                        ShiftName = model.Shift,
                        FromTime = DateTime.Parse(model.Date.ToDateMonthYearFormatedDate() + " " + model.FromTime),
                        ToTime = model.CrossDay
                        ? DateTime.Parse(model.Date.AddDays(1).ToDateMonthYearFormatedDate() + " " + model.ToTime)
                        : DateTime.Parse(model.Date.ToDateMonthYearFormatedDate() + " " + model.ToTime),
                        VerifierId = model.VerifierId,
                        ApproverId = model.ApproverId,
                        ModifiedBy = CurrentUser.Id,
                    };

                    CommandResult result = commandService.Update(criteria);
                    if (result.IsSuccessful)
                    {
                        keepInViewRequestService.SendMailForUpdateSuccessfully(criteria);
                        ViewBag.Updated = true;
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, result.Message);
                    }
                }
            }

            ViewBag.HasOnBehalfOthersPermission = CurrentUser.HasPermission(FunctionKey.CanOnBehalfOthers, Policy.Edit);

            return View(model);
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

            var criteria = new GetRequestCriteria()
            {
                EmpId = CurrentUser.Id,
                FromDate = new DateTime(year, 1, 1),
                ToDate = new DateTime(year, 12, 31),
                RequestType = (int)RequestFilter.KeepInView,
                IsQueryByCreatedDate = false,
                RequestViewType = RequestViewType.CompanySpace,
                SupervisorId = CurrentUser.Id,
                IsLocal = null
            };

            IList<RequestEntity> entityList = requestService.GetRequests(criteria);

            return View(entityList);
        }

        private IList<DateTime> GetUnavailDate(int employeeId)
        {
            var fromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-1);
            var toDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(2).AddDays(-1);
            var criteria = new GetOffDateAndHolidayDateCriteria()
            {
                EmpId = employeeId,
                FromDate = fromDate,
                ToDate = toDate
            };
            IList<DateTime> unavailableDates = employeeService.GetNotOffOrHolidayDate(criteria);

            return unavailableDates;
        }

        private VerifierAndApproverForRequest GetVerifierApprover(int employeeId)
        {
            var criteria = new GetVerifierAndApproverCriteria
            {
                EmpId = employeeId,
                RequestType = (byte)RequestType.KeepInView
            };
            VerifierAndApproverForRequest supervisor = requestService.GetVerifierAndApprover(criteria);
            return supervisor;
        }
    }
}
