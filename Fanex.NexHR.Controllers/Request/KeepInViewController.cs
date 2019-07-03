namespace Fanex.NexHR.Controllers.Request
{
    using System;
    using System.Web.Mvc;
    using Core.Models;
    using Core.Services;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Request.KeepInView;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels.Request;

    public class KeepInViewController : BaseController
    {
        private readonly IKeepInViewRequestService keepInViewRequestService;
        private readonly IRequestService requestService;
        private readonly IBaseService baseService;

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

        public ActionResult Create()
        {
            var model = new KeepInViewRequestViewModel
            {
                Employee = sessionService.Employee.Nickname,
                EmployeeId = sessionService.Employee.Id,
                Date = DateTime.Now.Date,
                KeepInViewType = (int)KeepInViewTypes.KeepInViewByDay,
                VerifierAndApprover = requestService.GetVerifierAndApprover(sessionService.Employee.Id, RequestType.KeepInView)
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
                model.VerifierAndApprover = requestService.GetVerifierAndApprover(model.EmployeeId, RequestType.KeepInView);
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
            var criteria = new GetKeepInViewRequestByIdCriteria
            {
                RequestId = id
            };
            KeepInViewModel request = keepInViewRequestService.GetById(criteria);
            if (request == null || !Utilities.CheckUserCanEditRequest(sessionService.Employee.Id, request.Employee.Id))
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
                    Date = request.OnDate,
                    KeepInViewType = request.KeepInViewTypeId,
                    ApproverId = request.ApprovedBy.Id,
                    VerifierId = request.VerifiedBy.Id,
                    Remark = request.Remark,
                    CrossDay = (request.ToTime - request.OnDate).Days > 0,
                    FromTime = request.FromTime.ToString("HH:mm"),
                    ToTime = request.ToTime.ToString("HH:mm"),
                    VerifierAndApprover = requestService.GetVerifierAndApprover(sessionService.Employee.Id, RequestType.KeepInView)
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
                model.VerifierAndApprover = requestService.GetVerifierAndApprover(model.EmployeeId, RequestType.KeepInView);

                if (ModelState.IsValid)
                {
                    var criteria = new UpdateKeepInViewRequestCriteria
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

        [HttpPost]
        public JsonResult Delete(int id)
        {
            var criteria = new DeleteKeepInViewRequestCriteria
            {
                RequestId = id
            };

            keepInViewRequestService.SendMailForDeleteSuccessfully(id);
            CommandResult result = baseService.ExecuteNonQuery(criteria);

            return Json(result);
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

            var criteria = new GetKeepInViewRequestByDateCriteria
            {
                EmpId = sessionService.Employee.Id,
                FromDate = new DateTime(year, 1, 1),
                ToDate = new DateTime(year, 12, 31)
            };

            var requests = keepInViewRequestService.GetByDate(criteria);
            return View(requests);
        }

        public ActionResult ViewHistory(long id)
        {
            var criteria = new GetKeepInViewRequestHistoryByIdCriteria
            {
                RequestId = id
            };
            var requests = keepInViewRequestService.GetHistoryById(criteria);
            return View(requests);
        }
    }
}