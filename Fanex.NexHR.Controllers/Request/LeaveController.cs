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
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// Leave Controller.
    /// </summary>
    public class LeaveController : RequestController
    {
        /// <summary>
        /// ActionResult Create leave request.
        /// </summary>
        /// <returns>View Result.</returns>
        public ActionResult Create()
        {
            var getLeaveBalanceCriteria = new GetLeaveBalanceCriteria()
            {
                EmpId = this.CurrentUser.Id,
                RequestId = null,
                EffectedDay = DateTime.Now.Date
            };
            EmployeeLeaveBalance employeeLeaveBalance = _LeaveRequestService.GetLeaveBalance(getLeaveBalanceCriteria);

            var currentDate = DateTime.Now.Date;
            var fromDate = new DateTime(currentDate.Year, currentDate.Month, 1);
            var toDate = new DateTime(currentDate.Year, currentDate.Month, DateTime.DaysInMonth(currentDate.Year, currentDate.Month));
            var requestInMonthCriteria = new GetNumberRequestInMonthCriteria()
            {
                EmployeeId = this.CurrentUser.Id,
                RequestType = (int)RequestFilter.Leave,
                FromDate = fromDate,
                ToDate = toDate
            };
            decimal numberRequests = _RequestService.GetNumberRequestInMonth(requestInMonthCriteria);

            var model = new LeaveRequestViewModel
            {
                LeaveTypeId = Constants.GeneralLeave,
                Employee = this.CurrentUser.DisplayName,
                EmployeeId = this.CurrentUser.Id,
                FromDate = DateTime.Now.Date,
                ToDate = DateTime.Now.Date,
                TotalDays = 1,
                LeaveTypeList = GetAllLeaveType(),
                TotalRequest = numberRequests,
                EmployeeLeaveBalance = employeeLeaveBalance
            };

            GetVerifierAndApprover(model);

            return View(model);
        }

        /// <summary>
        /// Function create new request.
        /// </summary>
        /// <param name="request">Format request string.</param>
        /// <param name="requestDetails">Format request detail string.</param>
        /// <returns>Action Result.</returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(string request, string requestDetails)
        {
            InsertLeaveRequestCriteria criteria = JsonConverter.FromJson<InsertLeaveRequestCriteria>(request);
            List<LeaveDetailModel> leaveRequestDetail = JsonConverter.FromJsonArray<LeaveDetailModel>(requestDetails);
            criteria.RequestDetail = leaveRequestDetail;
            criteria.CreatedBy = this.CurrentUser.Id;

            CommandResult result = _LeaveRequestService.Insert(criteria);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// ActionResult Update.
        /// </summary>
        /// <param name="requestId">WorkingTimeRequest id.</param>
        /// <returns>View update.</returns>
        public ActionResult Update(long requestId)
        {
            var criteria = new GetRequestByIdCriteria()
            {
                RequestId = requestId
            };
            RequestEntity request = _LeaveRequestService.GetRequestById(criteria);
            LeaveRequestViewModel model = ConvertRequestToModel(request);
            if (model == null || !Utilities.CheckUserCanEditRequest(this.CurrentUser.Id, model.EmployeeId))
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            var getLeaveBalanceCriteria = new GetLeaveBalanceCriteria()
            {
                EmpId = this.CurrentUser.Id,
                RequestId = requestId,
                EffectedDay = DateTime.Now.Date
            };
            EmployeeLeaveBalance employeeLeaveBalance = _LeaveRequestService.GetLeaveBalance(getLeaveBalanceCriteria);
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
                : _EmployeeService.GetUnavailableDates(getUnavailableDateCriteria);
            model.UnavailDates = listUnavailDates;
            model.LeaveTypeList = GetAllLeaveType();
            model.EmployeeLeaveBalance = employeeLeaveBalance;
            GetVerifierAndApprover(model);

            ViewBag.IsView = false;
            ViewBag.MinDate = minDate;
            ViewBag.MaxDate = maxDate;
            return View(model);
        }

        /// <summary>
        /// Function update leave request.
        /// </summary>
        /// <param name="request">Format request string.</param>
        /// <param name="requestDetails">Format request detail string.</param>
        /// <returns>Action Result.</returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Update(string request, string requestDetails)
        {
            UpdateLeaveRequestCriteria criteria = JsonConverter.FromJson<UpdateLeaveRequestCriteria>(request);
            List<LeaveDetailModel> leaveRequestDetail = JsonConverter.FromJsonArray<LeaveDetailModel>(requestDetails);
            criteria.RequestDetail = leaveRequestDetail;
            criteria.ModifiedBy = this.CurrentUser.Id;

            CommandResult result = _LeaveRequestService.Update(criteria);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// ActionResult Detail.
        /// </summary>
        /// <param name="requestId">WorkingTimeRequest id.</param>
        /// <returns>View Detail.</returns>
        public ActionResult Detail(int requestId)
        {
            var criteria = new GetRequestByIdCriteria()
            {
                RequestId = requestId
            };
            RequestEntity request = _LeaveRequestService.GetRequestById(criteria);
            LeaveRequestViewModel model = ConvertRequestToModel(request);
            model.LeaveTypeList = GetAllLeaveType();
            GetVerifierAndApprover(model);

            ViewBag.IsView = true;
            return View(model);
        }

        [HttpGet]
        public ActionResult EmployeeView()
        {
            return EmployeeView(DateTime.Now.Year);
        }

        /// <summary>
        /// ActionResult EmployeeView.
        /// </summary>
        /// <param name="year">Inputted year.</param>
        /// <returns>View Employee's Leave Request.</returns>
        [HttpPost]
        public ActionResult EmployeeView(int year)
        {
            var getLeaveBalanceCriteria = new GetLeaveBalanceCriteria()
            {
                EmpId = this.CurrentUser.Id,
                RequestId = null,
                EffectedDay = new DateTime(year, 1, 1)
            };
            EmployeeLeaveBalance employeeLeaveBalance = _LeaveRequestService.GetLeaveBalance(getLeaveBalanceCriteria);

            IList<RequestEntity> entityList = _RequestService.GetRequests(
                RequestViewType.CompanySpace,
                (int)RequestFilter.Leave,
                new DateTime(year, 1, 1),
                new DateTime(year, 12, 31),
                string.Empty,
                0,
                true,
                this.CurrentUser.Id,
                null,
                null);

            ViewBag.Year = year;
            ViewBag.EmployeeBalance = employeeLeaveBalance;
            return View(entityList);
        }

        public ActionResult GetLeaveBalance(int employeeId, long? requestId, DateTime? toDate)
        {
            var getLeaveBalanceCriteria = new GetLeaveBalanceCriteria()
            {
                EmpId = employeeId,
                RequestId = requestId,
                EffectedDay = toDate.HasValue ? toDate.Value : DateTime.Now.Date
            };
            EmployeeLeaveBalance employeeLeaveBalance = _LeaveRequestService.GetLeaveBalance(getLeaveBalanceCriteria);
            return Json(employeeLeaveBalance, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Action to get leave request detail base employeeId, FromDate, ToDate.
        /// </summary>
        /// <param name="employeeId">Employee Id.</param>
        /// <param name="fromDate">From Date.</param>
        /// <param name="toDate">The To Date.</param>
        /// <param name="requestId">Request Id.</param>
        /// <returns>Action Result.</returns>
        public ActionResult LeaveRequestDetail(int employeeId, DateTime fromDate, DateTime toDate, long requestId = -1)
        {
            IList<WorkingCalendarModel> workingCalendars = _WorkingCalendarService.GetWorkingCalendarsByDateRange(fromDate, toDate, string.Empty, employeeId);
            IList<LeaveDetailModel> leaveRequestDetailList = new List<LeaveDetailModel>();
            if (requestId != -1)
            {
                leaveRequestDetailList = _LeaveRequestService.GetLeaveRequestDetailsById(requestId);
            }

            leaveRequestDetailList = ConvertToLeaveRequestDetail(workingCalendars, leaveRequestDetailList, fromDate, toDate);
            var model = new LeaveDetailViewModel
            {
                LeaveRequestDetail = leaveRequestDetailList,
                LeaveTypeList = GetAllLeaveType()
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult SendReasonReeditToSupervisor(long requestId, string reason)
        {
            var errorMessage = string.Empty;
            var errorCode = 0;

            _LeaveRequestService.SendReasonReeditToSupervisor(requestId, this.CurrentUser.Id, reason, out errorCode, out errorMessage);

            return Json(new { errorCode = errorCode, errorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Function to convert from LeaveDetailModel get DB with WorkingCalendarModel.
        /// </summary>
        /// <param name="workingCalendars">List WorkingCalendarModel.</param>
        /// <param name="leaveRequestDetailInput">List LeaveDetailModel.</param>
        /// <param name="fromDate">From Date.</param>
        /// <param name="toDate">The To Date.</param>
        /// <returns>List LeaveDetail Model.</returns>
        [NonAction]
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

        /// <summary>
        /// Function to add a leave request detail with exist working calendar. 
        /// </summary>
        /// <param name="leaveRequestDetailOutput">Output List LeaveDetailModel.</param>
        /// <param name="tempLeaveDetailModels">Input List LeaveDetailModel.</param>
        /// <param name="tempWokingCalendar">WorkingCalendar Model.</param>
        [NonAction]
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

        /// <summary>
        /// Function to add a leave request detail with not exist working calendar.
        /// </summary>
        /// <param name="leaveRequestDetailOutput">Output List LeaveDetailModel.</param>
        /// <param name="tempLeaveDetailModels">Input List LeaveDetailModel.</param>
        /// <param name="workDate">Input the work date.</param>
        [NonAction]
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

        [NonAction]
        private IList<SelectListItem> GetListItem(IList<SupervisorNameModel> supervisors)
        {
            IList<SelectListItem> selectItems = supervisors.Select(x => new SelectListItem
            {
                Text = x.NickName + " (" + x.FullName + ")",
                Value = x.OwnerId.ToString()
            }).ToList();

            return selectItems;
        }

        [NonAction]
        private void GetVerifierAndApprover(LeaveRequestViewModel model)
        {
            IList<SupervisorNameModel> verifiers = this.GetSupervisorByRequestType(this.CurrentUser.Id, (byte)RoleSystem.Verifier, (byte)RequestType.Leave);
            IList<SupervisorNameModel> approvers = this.GetSupervisorByRequestType(this.CurrentUser.Id, (byte)RoleSystem.Verifier, (byte)RequestType.Leave);
            model.VerifierList = GetListItem(verifiers);
            model.ApproverList = GetListItem(approvers);
        }

        [NonAction]
        private IList<LeaveType> GetAllLeaveType()
        {
            var criteria = new GetLeaveTypeCriteria()
            {
                ExcludeNotShown = true
            };
            IList<LeaveType> leaveTypes = _LeaveTypeService.GetAll(criteria);
            return leaveTypes;
        }

        /// <summary>
        /// Function to convert from request entity to leave request view model.
        /// </summary>
        /// <param name="request">Request Entity.</param>
        /// <returns>LeaveRequest ViewModel.</returns>
        [NonAction]
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
    }
}
