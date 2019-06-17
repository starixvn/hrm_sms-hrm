namespace Fanex.NexHR.Controllers.Request
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Core.Models;
    using Core.Services;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Criteria.Request.KeepInView;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels.Request;

    public class KeepInViewController : BaseController
    {
        private readonly IKeepInViewRequestService keepInViewRequestService;
        private readonly IRequestService requestService;
        private readonly IBaseService baseService;
        private readonly IShiftService shiftService;

        public KeepInViewController(
            ISessionService sessionService,
            IKeepInViewRequestService keepInViewRequestService,
            IRequestService requestService,
            IBaseService baseService,
            IShiftService shiftService) : base(sessionService)
        {
            this.keepInViewRequestService = keepInViewRequestService;
            this.requestService = requestService;
            this.baseService = baseService;
            this.shiftService = shiftService;
        }

        public ActionResult Create()
        {
            Shift shift = shiftService.GetByDate(sessionService.Employee.Id, DateTime.Now);

            var model = new KeepInViewRequestViewModel
            {
                Employee = sessionService.Employee.Nickname,
                EmployeeId = sessionService.Employee.Id,
                Date = DateTime.Now.Date,
                Shift = (shift == null) ? string.Empty : shift.Name,
                ShiftId = (shift == null) ? 0 : shift.Id,
                VerifierAndApprover = GetVerifierApprover(sessionService.Employee.Id)
            };
            ViewBag.HasOnBehalfOthersPermission = sessionService.HasPermissionOnFunction(FunctionKey.CanOnBehalfOthers, Policy.Read)
                || sessionService.HasPermissionOnFunction(FunctionKey.CanOnBehalfOthers, Policy.Host);

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(KeepInViewRequestViewModel model)
        {
            if (model != null)
            {
                model.VerifierAndApprover = GetVerifierApprover(model.EmployeeId);
                var fromTime = DateTime.Parse(model.Date.ToDateMonthYearFormatedDate() + " " + model.FromTime);
                var toTime = DateTime.Parse(model.Date.ToDateMonthYearFormatedDate() + " " + model.ToTime);

                if (ModelState.IsValid)
                {
                    var criteria = new InsertKeepInViewRequestCriteria
                    {
                        EmpId = model.EmployeeId,
                        OnDate = model.Date,
                        KeepInViewType = model.KeepInViewType,
                        IsHalfDay = model.HalfDay,
                        RefNo = model.RefNo,
                        Remark = model.Remark,
                        ShiftName = model.Shift,
                        FromTime = (fromTime < toTime && model.CrossDay) ? DateTime.Parse(model.Date.AddDays(1).ToDateMonthYearFormatedDate() + " " + model.FromTime)
                            : DateTime.Parse(model.Date.ToDateMonthYearFormatedDate() + " " + model.FromTime),
                        ToTime = model.CrossDay
                        ? DateTime.Parse(model.Date.AddDays(1).ToDateMonthYearFormatedDate() + " " + model.ToTime)
                        : DateTime.Parse(model.Date.ToDateMonthYearFormatedDate() + " " + model.ToTime),
                        VerifierId = model.VerifierId,
                        ApproverId = model.ApproverId,
                        CreatedBy = sessionService.Employee.Id
                    };

                    CommandResult result = baseService.ExecuteNonQuery(criteria, hasGetOutputParams: true);
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

            ViewBag.HasOnBehalfOthersPermission = sessionService.HasPermissionOnFunction(FunctionKey.CanOnBehalfOthers, Policy.Read)
                || sessionService.HasPermissionOnFunction(FunctionKey.CanOnBehalfOthers, Policy.Host);

            return View(model);
        }

        public ActionResult Update(long id)
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
                var viewModel = new KeepInViewRequestViewModel
                {
                    RequestId = id,
                    RefNo = request.RefNO,
                    HalfDay = request.IsHalfDay,
                    Employee = sessionService.Employee.Nickname,
                    EmployeeId = sessionService.Employee.Id,
                    Date = request.RequestedDate,
                    Shift = request.FromShift.Name,
                    ShiftId = request.FromShift.Id,
                    ApproverId = request.ApprovedBy.Id,
                    VerifierId = request.VerifiedBy.Id,
                    Remark = request.Remark,
                    CrossDay = request.CrossDay,
                    FromTime = request.FromTime.ToString("HH:mm"),
                    ToTime = request.ToTime.ToString("HH:mm"),
                    VerifierAndApprover = GetVerifierApprover(sessionService.Employee.Id)
                };
                ViewBag.HasOnBehalfOthersPermission = sessionService.HasPermissionOnFunction(FunctionKey.CanOnBehalfOthers, Policy.Read)
                    || sessionService.HasPermissionOnFunction(FunctionKey.CanOnBehalfOthers, Policy.Host);

                return View(viewModel);
            }
        }

        [HttpPost]
        public ActionResult Update(KeepInViewRequestViewModel model)
        {
            if (model != null)
            {
                model.VerifierAndApprover = GetVerifierApprover(model.EmployeeId);

                if (ModelState.IsValid)
                {
                    var criteria = new UpdateKeepInViewRequestCriteria
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
                        ModifiedBy = sessionService.Employee.Id,
                    };

                    CommandResult result = baseService.ExecuteNonQuery(criteria);
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

            ViewBag.HasOnBehalfOthersPermission = sessionService.HasPermissionOnFunction(FunctionKey.CanOnBehalfOthers, Policy.Read)
                || sessionService.HasPermissionOnFunction(FunctionKey.CanOnBehalfOthers, Policy.Host);

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

            var criteria = new GetRequestCriteria
            {
                EmpId = sessionService.Employee.Id,
                FromDate = new DateTime(year, 1, 1),
                ToDate = new DateTime(year, 12, 31),
                RequestType = (int)RequestFilter.KeepInView,
                IsQueryByCreatedDate = false,
                RequestViewType = RequestViewType.CompanySpace,
                SupervisorId = sessionService.Employee.Id,
                IsLocal = null
            };

            IList<RequestEntity> entityList = requestService.GetRequests(criteria);

            return View(entityList);
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