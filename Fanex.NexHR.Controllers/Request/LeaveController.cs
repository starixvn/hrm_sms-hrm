// --------------------------------------------------------------------------
// <copyright file="LeaveController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Controllers.Request
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Calendar;
    using Fanex.NexHR.Criteria.LeaveType;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Criteria.Request.Leave;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Calendar;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Models.Supervisor;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.ViewModels.Request;

    public class LeaveController : RequestController
    {
        public ActionResult Create()
        {
            var model = new LeaveRequestViewModel
            {
                LeaveTypeId = Constants.GeneralLeave,
                Employee = CurrentUser.DisplayName,
                EmployeeId = CurrentUser.Id,
                FromDate = DateTime.Now.Date,
                ToDate = DateTime.Now.Date,
                TotalDays = 1,
                LeaveTypeList = GetAllLeaveType(),
                TotalRequest = GetNumberRequestInMonth(),
                EmployeeLeaveBalance = GetLeaveBalance(null, null),
                VerifierList = GetVerifiers(CurrentUser.Id),
                ApproverList = GetApprovers(CurrentUser.Id)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(string request, string requestDetails)
        {
            InsertLeaveRequestCriteria criteria = JsonConverter.FromJson<InsertLeaveRequestCriteria>(request);
            List<LeaveDetailModel> leaveRequestDetail = JsonConverter.FromJsonArray<LeaveDetailModel>(requestDetails);
            criteria.RequestDetail = leaveRequestDetail;
            criteria.CreatedBy = CurrentUser.Id;
            criteria.CreatedDate = DateTime.Now;

            CommandResult result = LeaveRequestService.Insert(criteria);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Update(long requestId)
        {
            var criteria = new GetRequestByIdCriteria()
            {
                RequestId = requestId
            };
            RequestEntity request = RequestService.GetRequestById(criteria);
            LeaveRequestViewModel model = ConvertRequestToModel(request);
            if (model == null || !Utilities.CheckUserCanEditRequest(CurrentUser.Id, model.EmployeeId))
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            var minDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-1);
            var maxDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(Constants.NumberMonthsInFutureCanLeaveRequest).AddDays(-1);
            minDate = minDate > model.FromDate ? model.FromDate : minDate;
            if (model.LeaveTypeId == Constants.MaternityLeave)
            {
                //// rule: user can make leave request (maternity leave) for 6 months.
                maxDate = maxDate.AddMonths(2);
            }

            var getUnavailableDateCriteria = new GetUnavailableRequestDateCriteria()
            {
                EmpId = this.CurrentUser.Id,
                FromDate = minDate,
                ToDate = maxDate
            };

            IList<DateTime> listUnavailDates = model.LeaveTypeId == Constants.GeneralLeave
                ? null
                : EmployeeService.GetUnavailableDates(getUnavailableDateCriteria);
            model.UnavailDates = listUnavailDates;

            model.LeaveTypeList = GetAllLeaveType();
            model.EmployeeLeaveBalance = GetLeaveBalance(requestId, null);
            model.VerifierList = GetVerifiers(CurrentUser.Id);
            model.ApproverList = GetApprovers(CurrentUser.Id);

            ViewBag.IsView = false;
            ViewBag.MinDate = minDate;
            ViewBag.MaxDate = maxDate;
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Update(string request, string requestDetails)
        {
            UpdateLeaveRequestCriteria criteria = JsonConverter.FromJson<UpdateLeaveRequestCriteria>(request);
            List<LeaveDetailModel> leaveRequestDetail = JsonConverter.FromJsonArray<LeaveDetailModel>(requestDetails);
            criteria.RequestDetail = leaveRequestDetail;
            criteria.ModifiedBy = CurrentUser.Id;
            criteria.ModifiedDate = DateTime.Now;

            CommandResult result = LeaveRequestService.Update(criteria);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Detail(int requestId)
        {
            var criteria = new GetRequestByIdCriteria()
            {
                RequestId = requestId
            };
            RequestEntity request = RequestService.GetRequestById(criteria);
            LeaveRequestViewModel model = ConvertRequestToModel(request);
            model.LeaveTypeList = GetAllLeaveType();
            model.VerifierList = GetVerifiers(request.Requester.Id);
            model.ApproverList = GetApprovers(request.Requester.Id);

            ViewBag.IsView = true;
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
            var getLeaveBalanceCriteria = new GetLeaveBalanceCriteria()
            {
                EmpId = CurrentUser.Id,
                RequestId = null,
                EffectedDay = new DateTime(year, 1, 1)
            };
            EmployeeLeaveBalance employeeLeaveBalance = LeaveRequestService.GetLeaveBalance(getLeaveBalanceCriteria);
            var criteria = new GetRequestCriteria()
            {
                SupervisorId = CurrentUser.Id,
                EmpId = CurrentUser.Id,
                FromDate = new DateTime(year, 1, 1),
                ToDate = new DateTime(year, 12, 31),
                RequestType = (int)RequestFilter.Leave,
                IsQueryByCreatedDate = false,
                Role = CheckRoleAccordingToRequestType(RequestViewType.CompanySpace),
                IsLocal = null
            };

            IList<RequestEntity> entityList = RequestService.GetRequests(criteria);

            ViewBag.Year = year;
            ViewBag.EmployeeBalance = GetLeaveBalance(null, year);
            return View(entityList);
        }

        public ActionResult GetLeaveBalance(int employeeId, long? requestId, DateTime? toDate)
        {
            var getLeaveBalanceCriteria = new GetLeaveBalanceCriteria()
            {
                EmpId = employeeId,
                RequestId = requestId == -1 ? null : requestId,
                EffectedDay = toDate.HasValue ? toDate.Value : DateTime.Now.Date
            };
            EmployeeLeaveBalance employeeLeaveBalance = LeaveRequestService.GetLeaveBalance(getLeaveBalanceCriteria);
            return Json(employeeLeaveBalance, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LeaveRequestDetail(int employeeId, DateTime fromDate, DateTime toDate, long requestId = -1)
        {
            var criteria = new GetWorkingCalendarByDateRangeCriteria()
            {
                FromDate = fromDate,
                ToDate = toDate,
                GroupIds = string.Empty,
                EmpId = employeeId
            };
            IList<WorkingCalendarModel> calendars = WorkingCalendarService.GetWorkingCalendarByDateRange(criteria);
            IList<LeaveDetailModel> leaveRequestDetail = new List<LeaveDetailModel>();
            if (requestId != -1)
            {
                var leaveDetailCriteria = new GetLeaveRequestDetailByIdCriteria()
                {
                    RequestId = requestId
                };

                leaveRequestDetail = LeaveRequestService.GetLeaveRequestDetailById(leaveDetailCriteria);
            }

            leaveRequestDetail = ConvertToLeaveRequestDetail(calendars, leaveRequestDetail, fromDate, toDate);
            var model = new LeaveDetailViewModel
            {
                LeaveRequestDetail = leaveRequestDetail,
                LeaveTypeList = GetAllLeaveType()
            };

            return View(model);
        }

        public ActionResult SendReasonReeditToSupervisor(long requestId, string reason)
        {
            var criteria = new ResetLeaveRequestCriteria() 
            { 
                RequestId = requestId,
                EmpId = CurrentUser.Id,
                Reason = reason
            };
            CommandResult result = LeaveRequestService.SendReasonReeditToSupervisor(criteria);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private List<LeaveDetailModel> ConvertToLeaveRequestDetail(
            IList<WorkingCalendarModel> workingCalendars,
            IList<LeaveDetailModel> leaveRequestDetailInput,
            DateTime fromDate,
            DateTime toDate)
        {
            var leaveRequestDetailOutput = new List<LeaveDetailModel>();
            if (fromDate <= toDate)
            {
                var numberDays = (toDate - fromDate).Days + 1;
                var tempDate = DateTime.Now.Date;
                IEnumerable<WorkingCalendarModel> tempWokingCalendars = null;
                IEnumerable<LeaveDetailModel> tempLeaveDetailModels = null;
                WorkingCalendarModel tempWokingCalendar = null;

                for (int i = 0; i < numberDays; i++)
                {
                    tempLeaveDetailModels = null;
                    tempDate = fromDate.AddDays(i);
                    tempLeaveDetailModels = leaveRequestDetailInput.Where(t => (t.LeaveDate == tempDate));
                    tempWokingCalendars = workingCalendars.Where(t => t.WorkDate.Date == tempDate);

                    if (tempWokingCalendars != null && tempWokingCalendars.Any())
                    {
                        tempWokingCalendar = tempWokingCalendars.FirstOrDefault();
                        AddLeaveRequestExistCalendar(leaveRequestDetailOutput, tempLeaveDetailModels, tempWokingCalendar);
                    }
                    else
                    {
                        AddLeaveRequestNotExistCalendar(leaveRequestDetailOutput, tempLeaveDetailModels, tempDate);
                    }
                }
            }

            return leaveRequestDetailOutput.OrderBy(t => t.LeaveDate).ThenBy(t => t.FromTime).ToList();
        }

        private void AddLeaveRequestExistCalendar(
           List<LeaveDetailModel> leaveRequestDetailOutput,
           IEnumerable<LeaveDetailModel> tempLeaveDetailModels,
           WorkingCalendarModel tempWokingCalendar)
        {
            string tempScheduleStatus = string.Empty;
            LeaveDetailModel tempLeaveDetailModel = null;
            var isOffOrHoliday = tempWokingCalendar.IsOffDay || tempWokingCalendar.IsHoliday;

            if (tempWokingCalendar.IsOffDay)
            {
                tempScheduleStatus = Labels.OffDay;
            }
            else if (tempWokingCalendar.IsHoliday)
            {
                tempScheduleStatus = Labels.Holiday;
            }
            else if (tempWokingCalendar.Shift != null)
            {
                tempScheduleStatus = tempWokingCalendar.Shift.Name;
            }
            else if (!tempWokingCalendar.IsLeaveDay)
            {
                tempScheduleStatus = Labels.NA;
            }

            if (tempLeaveDetailModels != null && tempLeaveDetailModels.Any())
            {
                foreach (LeaveDetailModel leaveDetail in tempLeaveDetailModels)
                {
                    tempLeaveDetailModel = leaveDetail;
                    tempLeaveDetailModel.IsOffOrHoliday = isOffOrHoliday;
                    tempLeaveDetailModel.ScheduleStatus = tempScheduleStatus;
                    tempLeaveDetailModel.IsFromAfterday = leaveDetail.FromTime.Date > leaveDetail.LeaveDate.Date;
                    tempLeaveDetailModel.IsToAfterday = leaveDetail.ToTime.Date > leaveDetail.LeaveDate.Date;
                    leaveRequestDetailOutput.Add(tempLeaveDetailModel);
                }
            }
            else
            {
                tempLeaveDetailModel = new LeaveDetailModel
                {
                    LeaveDate = tempWokingCalendar.WorkDate,
                    ScheduleStatus = tempScheduleStatus,
                    IsOffOrHoliday = isOffOrHoliday,
                    LeaveTypeId = Constants.AnnualLeave,
                    WorkType = 2
                };
                leaveRequestDetailOutput.Add(tempLeaveDetailModel);
            }
        }

        private void AddLeaveRequestNotExistCalendar(
            List<LeaveDetailModel> leaveRequestDetailOutput,
            IEnumerable<LeaveDetailModel> tempLeaveDetailModels,
            DateTime workDate)
        {
            string tempScheduleStatus = Labels.NA;
            LeaveDetailModel tempLeaveDetailModel = null;

            if (tempLeaveDetailModels != null && tempLeaveDetailModels.Any())
            {
                foreach (LeaveDetailModel leaveDetail in tempLeaveDetailModels)
                {
                    tempLeaveDetailModel = leaveDetail;
                    tempLeaveDetailModel.ScheduleStatus = tempScheduleStatus;
                    tempLeaveDetailModel.IsFromAfterday = leaveDetail.FromTime.Date > leaveDetail.LeaveDate.Date;
                    tempLeaveDetailModel.IsToAfterday = leaveDetail.ToTime.Date > leaveDetail.LeaveDate.Date;
                    leaveRequestDetailOutput.Add(tempLeaveDetailModel);
                }
            }
            else
            {
                tempLeaveDetailModel = new LeaveDetailModel
                {
                    LeaveDate = workDate,
                    ScheduleStatus = Labels.NA,
                    LeaveTypeId = Constants.AnnualLeave,
                    WorkType = 2
                };
                leaveRequestDetailOutput.Add(tempLeaveDetailModel);
            }
        }

        private IList<SelectListItem> GetListItem(IList<SupervisorNameModel> supervisors)
        {
            IList<SelectListItem> selectItems = supervisors.Select(x => new SelectListItem
            {
                Text = x.NickName + " (" + x.FullName + ")",
                Value = x.OwnerId.ToString()
            }).ToList();

            return selectItems;
        }

        private IList<SelectListItem> GetVerifiers(int empId)
        {
            IList<SupervisorNameModel> verifiers = GetSupervisorByRequestType(empId, (byte)RoleSystem.Verifier, (byte)RequestType.Leave);
            return GetListItem(verifiers);
        }

        private IList<SelectListItem> GetApprovers(int empId)
        {
            IList<SupervisorNameModel> approvers = GetSupervisorByRequestType(empId, (byte)RoleSystem.Approver, (byte)RequestType.Leave);
            return GetListItem(approvers);
        }

        private IList<LeaveType> GetAllLeaveType()
        {
            var criteria = new GetLeaveTypeCriteria()
            {
                ExcludeNotShown = true
            };
            IList<LeaveType> leaveTypes = LeaveTypeService.GetAll(criteria);
            return leaveTypes;
        }

        private LeaveRequestViewModel ConvertRequestToModel(RequestEntity request)
        {
            LeaveRequestViewModel model = null;
            if (request != null)
            {
                model = new LeaveRequestViewModel
                {
                    RequestId = request.Id,
                    LeaveTypeId = request.LeaveType.Id,
                    Employee = request.Requester.DisplayName,
                    EmployeeId = request.Requester.Id,
                    FromDate = request.FromDate,
                    ToDate = request.ToDate,
                    TotalDays = request.TotalDays,
                    HalfDayOff = request.IsHalfDay,
                    VerifierId = request.VerifiedBy.Id,
                    ApproverId = request.ApprovedBy.Id,
                    RefNo = request.RefNO,
                    Remark = request.Remark,
                    FromTime = request.FromTime.ToString("HH:mm"),
                    ToTime = request.ToTime.ToString("HH:mm"),
                    IsFromTimeAfterDay = request.FromDate.Date != request.FromTime.Date,
                    IsToTimeAfterDay = request.ToDate.Date != request.ToTime.Date,
                    Status = request.Status
                };
            }

            return model;
        }

        private EmployeeLeaveBalance GetLeaveBalance(long? requestId, int? year)
        {
            var getLeaveBalanceCriteria = new GetLeaveBalanceCriteria()
            {
                EmpId = CurrentUser.Id,
                RequestId = requestId,
                EffectedDay = year.HasValue ? new DateTime(year.Value, 1, 1) : DateTime.Now.Date
            };

            EmployeeLeaveBalance employeeLeaveBalance = LeaveRequestService.GetLeaveBalance(getLeaveBalanceCriteria);
            return employeeLeaveBalance;
        }

        private decimal GetNumberRequestInMonth()
        {
            var currentDate = DateTime.Now.Date;
            var fromDate = new DateTime(currentDate.Year, currentDate.Month, 1);
            var toDate = new DateTime(currentDate.Year, currentDate.Month, DateTime.DaysInMonth(currentDate.Year, currentDate.Month));
            var requestInMonthCriteria = new GetNumberRequestInMonthCriteria()
            {
                EmployeeId = CurrentUser.Id,
                RequestType = (int)RequestType.Leave,
                FromDate = fromDate,
                ToDate = toDate
            };
            decimal numberRequests = RequestService.GetNumberRequestInMonth(requestInMonthCriteria);

            return numberRequests;
        }
    }
}
