// --------------------------------------------------------------------------
// <copyright file="OvertimeController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.9.3">Created</change>
// </history>

namespace Fanex.NexHR.Controllers.Request
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interface;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// Overtime Controller.
    /// </summary>
    public class OvertimeController : RequestController
    {
        private readonly ILookupDataService _lookupDataService;

        public OvertimeController()
            : this(null)
        {
        }

        public OvertimeController(ILookupDataService lookupDataService = null)
        {
            _lookupDataService = lookupDataService ?? new LookupDataService();
        }

        [HttpGet]
        public ActionResult EmployeeView()
        {
            var id = this.CurrentUser.Id;
            var employee = _EmployeeService.GetEmployeeById(id);
            var overtimeReportViewModel = new OvertimeReportViewModel()
            {
                Employee = employee.NickName,
                EmpId = employee.Id
            };
            return EmployeeView(overtimeReportViewModel, DateTime.Now.Year);
        }

        /// <summary>
        /// ActionResult EmployeeView.
        /// </summary>
        /// <param name="model">OvertimeReport ViewModel.</param>
        /// <param name="year">Inputted Year.</param>
        /// <returns>View Employee's Overtime Request.</returns>
        [HttpPost]
        public ActionResult EmployeeView(OvertimeReportViewModel model, int year = 0)
        {
            var currentYear = year != 0 ? year : DateTime.Now.Year;
            ViewBag.Year = currentYear;
            var entityList = _OvertimeService.GetOvertimeReportInYear(model.EmpId, currentYear);
            model.TotalRequest = entityList;

            return View(model);
        }

        /// <summary>
        /// View Overtime Request Details by Month.
        /// </summary>
        /// <param name="requestDate">Requested Date.</param>
        /// <param name="empId">Employee id.</param>
        /// <returns>View details.</returns>
        public ActionResult View(DateTime requestDate, int empId)
        {
            IList<OvertimeRequest> entityList = _OvertimeService.GetOvertimeRequests(empId, Convert.ToDateTime(requestDate).Date);
            var employee = _EmployeeService.GetEmployeeById(empId);
            var entity = new OvertimeRequest();
            if (entityList != null)
            {
                entity = entityList.Where(x => x.RequestId > 0).Take(1).FirstOrDefault();
            }

            var requestId = entity != null ? entity.RequestId : 0;
            var overtimeMonthlyRequest = _OvertimeService.GetOvertimeRequestById(requestId);

            var overtimeViewModel = new MyOverTimeViewModel()
            {
                OvertimeRequest = entityList,
                EmpId = empId,
                OvertimeRequestStatus = overtimeMonthlyRequest != null ? overtimeMonthlyRequest.Status.Name : string.Empty
            };
            ViewBag.RequestedDate = requestDate;
            ViewBag.Update = false;
            ViewBag.Employee = employee.DisplayName;
            GetVerifierApproverList(empId, overtimeViewModel);

            return View("ViewDetails", overtimeViewModel);
        }

        /// <summary>
        /// Action result update.
        /// </summary>
        /// <param name="requestDate">Request date.</param>
        /// <param name="empId">Employee id.</param>
        /// <returns>View update overtime.</returns>
        public ActionResult Update(DateTime requestDate, int empId)
        {
            IList<OvertimeRequest> entityList = _OvertimeService.GetOvertimeRequests(empId, Convert.ToDateTime(requestDate).Date);
            var employeeInfo = _EmployeeService.GetEmployeeById(empId);
            var entity = new OvertimeRequest();
            if (entityList != null)
            {
                entity = entityList.Where(x => x.RequestId > 0).Take(1).FirstOrDefault();
            }

            var requestId = entity != null ? entity.RequestId : 0;
            var overtimeMonthlyRequest = _OvertimeService.GetOvertimeRequestById(requestId);
            var overtimeViewModel = new MyOverTimeViewModel()
            {
                OvertimeRequest = entityList,
                VerifierId = overtimeMonthlyRequest != null ? overtimeMonthlyRequest.VerifiedBy.Id : 0,
                ApproverId = overtimeMonthlyRequest != null ? overtimeMonthlyRequest.ApprovedBy.Id : 0,
                EmpId = empId,
                OvertimeRequestStatus = overtimeMonthlyRequest != null ? overtimeMonthlyRequest.Status.Name : "Pending"
            };
            
            ViewBag.RequestStatus = overtimeViewModel.OvertimeRequestStatus;
            ViewBag.WorkingHours = employeeInfo.WorkingHours;
            ViewBag.RequestedDate = requestDate;
            ViewBag.Update = true;
            ViewBag.Employee = employeeInfo.DisplayName;
            GetVerifierApproverList(empId, overtimeViewModel);

            return View(overtimeViewModel);
        }

        /// <summary>
        /// Update Overtime Request in current month.
        /// </summary>
        /// <param name="overtimeJson">Overtime object from Overtime.</param>
        /// <param name="overtimeDetailsJson">Overtime Details object from overtimeDetails.</param>
        /// <returns>Boolean to check result.</returns>
        [HttpPost]
        public JsonResult Update(string overtimeJson, string overtimeDetailsJson)
        {
            var id = this.CurrentUser.Id;
            var model = JsonConverter.JsonExtensionMethod.FromJson<MyOverTimeViewModel>(overtimeJson);
            var overtimeDetailModel = JsonConverter.JsonExtensionMethod.FromJsonArray<OvertimeRequest>(overtimeDetailsJson).ToList();
            model.ProcessedBy = id;
            model.ProcessedDate = DateTime.Now.Date;
            model.OvertimeRequest = overtimeDetailModel.Where(x => x.TotalWorkHours > 0).ToList();
            var updated = _OvertimeService.UpdateOvertimeRequest(model, ModelState);
            if (!updated)
            {
                foreach (var item in ModelState.Values)
                {
                    foreach (ModelError error in item.Errors)
                    {
                        return Json(new { success = false, errors = error.ErrorMessage });
                    }
                }
            }

            return Json(new { success = true });
        }

        public ActionResult GetActingEmployees(string query)
        {
            var employees = _RequestService.GetSupervisorByQuery(this.CurrentUser.Id, (int)RolesEnum.ActingSupervisor, null, query.Trim(), includeLoginUser: true);

            return Json(employees, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "HR")]
        public ActionResult YearlyReport()
        {
            ViewBag.Year = DateTime.Now.Year;
            ViewBag.Groups = _TeamService.GetAll();

            return View();
        }

        /// <summary>
        /// Method Get Yearly Report By HR.
        /// </summary>
        /// <param name="year">Date time Year.</param>
        /// <param name="team">Team filter.</param>
        /// <param name="empId">Employee's id.</param>
        /// <param name="calendar">Fiscal or Normal.</param>
        /// <returns>List of OvertimeAnnualReport entity.</returns>
        [Authorize(Roles = "HR")]
        public ActionResult GetYearlyOvertimeReport(int year, string team, int? empId, int? calendar)
        {
            var currentUserId = this.CurrentUser.Id;
            ViewBag.Year = year;
            ViewBag.Calendar = calendar;
            IList<OvertimeAnnualReport> listEntity = null;

            if (calendar == 1)
            {
                listEntity = this._OvertimeService.GetYearlyOvertimeReport(year, team, empId, currentUserId, null);
            }
            else
            {
                listEntity = this._OvertimeService.GetYearlyOvertimeReportByNormalCalendar(year, team, empId, currentUserId, null);
            }

            return PartialView("YearlyReportDetails", listEntity);
        }

        /// <summary>
        /// ActionResult HRView Details.
        /// </summary>
        /// <param name="requestDate">Requested Date.</param>
        /// <param name="empId">Employee Id.</param>
        /// <param name="calendar">Fiscal or normal calendar.</param>
        /// <returns>View HRViewDetails.</returns>
        [Authorize(Roles = "HR")]
        public ActionResult HRViewDetails(DateTime requestDate, int empId, int? calendar)
        {
            var id = this.CurrentUser.Id;
            IList<OvertimeRequest> entityList = new List<OvertimeRequest>();
            if (calendar == 0)
            {
                entityList = _OvertimeService.HRGetNormalOvertimeRequest(id, empId, Convert.ToDateTime(requestDate).Date);
            }
            else
            {
                entityList = _OvertimeService.HRGetOvertimeRequest(id, empId, Convert.ToDateTime(requestDate).Date);
            }

            var employee = _EmployeeService.GetEmployeeById(empId);
            var overtimeViewModel = new MyOverTimeViewModel()
            {
                OvertimeRequest = entityList,
                EmpId = empId,
                Employee = employee.DisplayName
            };
            ViewBag.RequestedDate = requestDate;
            ViewBag.Update = false;
            GetVerifierApproverList(empId, overtimeViewModel);
            return View(overtimeViewModel);
        }

        /// <summary>
        /// Method HR Get Monthly Overtime Report.
        /// </summary>
        /// <param name="month">Input month.</param>
        /// <param name="year">Input year.</param>
        /// <param name="calendar">Fiscal or normal calendar.</param>
        /// <returns>View HRMonthlyReport.</returns>
        [Authorize(Roles = "HR")]
        public ActionResult HRMonthlyReport(int month, int year, int? calendar)
        {
            var date = new DateTime(year, month, 1);
            ViewBag.Date = date.ToString("MMM-yyyy");
            ViewBag.Calendar = calendar;

            if (calendar == 0)
            {
                var fromDate = new DateTime(year, month, 1);
                var toDate = new DateTime(year, month, DateTime.DaysInMonth(year, month));
                ViewBag.FromDate = fromDate;
                ViewBag.ToDate = toDate;
            }
            else
            {
                var currentDate = new DateTime(year, month, DateTime.Now.Day);
                var requestDatePreviousOneMonth = currentDate.AddMonths(-1).Date;
                var fromDate = new DateTime(requestDatePreviousOneMonth.Year, requestDatePreviousOneMonth.Month, 10);
                var toDate = new DateTime(year, month, 9);
                ViewBag.FromDate = fromDate;
                ViewBag.ToDate = toDate;
            }

            return View();
        }

        /// <summary>
        /// Method Get HRMonthlyReportDetails.
        /// </summary>
        /// <param name="fromDate">Get from date.</param>
        /// <param name="toDate">Get to date.</param>
        /// <param name="calendar">Fiscal or normal calendar.</param>
        /// <returns>View HRMonthlyReportDetails.</returns>
        public ActionResult HRMonthlyReportDetails(DateTime fromDate, DateTime toDate, int? calendar)
        {
            ViewBag.Calendar = calendar;
            if (calendar == 0)
            {
                var entityList = _OvertimeService.GetMonthlyOvertimeReportByNormalCalendar(fromDate, toDate, null, string.Empty, null);
                return PartialView(entityList);
            }
            else
            {
                var date = new DateTime(toDate.Year, toDate.Month, 1);
                var entityList = _OvertimeService.GetMonthlyOvertimeReport(date, null, string.Empty, null);
                return PartialView(entityList);
            }
        }

        [Authorize(Roles = "Verifier, Approver")]
        public ActionResult MemberOvertime()
        {
            var memberOvertimeViewModel = GetMemberOvertimeViewModel();
            memberOvertimeViewModel.IsHRView = false;

            return View(memberOvertimeViewModel);
        }

        [Authorize(Roles = "HR")]
        public ActionResult EmployeeOvertime()
        {
            var employeeOvertimeViewModel = GetMemberOvertimeViewModel();
            employeeOvertimeViewModel.IsHRView = true;

            return View("MemberOvertime", employeeOvertimeViewModel);
        }

        /// <summary>
        /// Member overtimes request view by Verifier or Approver.
        /// </summary>
        /// <param name="month">Month which overtime follow its.</param>
        /// <param name="employeeId">Employee Id.</param>
        /// <param name="teamIds">List string TeamIds.</param>
        /// <param name="statusId">Status Id.</param>
        /// <returns>Action Result.</returns>
        [Authorize(Roles = "Verifier, Approver")]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult MemberOvertimeViewByLeader(DateTime? month, int? employeeId, string teamIds, string statusId)
        {
            if (!month.HasValue)
            {
                month = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            }

            var monthlyOvertimes = _OvertimeService.GetMemberOvertimeMonthly(this.CurrentUser.Id, "2,3", month.Value, employeeId, teamIds, statusId);

            ViewBag.CurrentUserId = this.CurrentUser.Id;
            return View(monthlyOvertimes);
        }

        /// <summary>
        /// Employee overtimes request view by HR.
        /// </summary>
        /// <param name="month">Month which overtime follow its.</param>
        /// <param name="employeeId">Employee Id.</param>
        /// <param name="teamIds">List string TeamIds.</param>
        /// <param name="statusId">Status Id.</param>
        /// <returns>Action Result.</returns>
        [Authorize(Roles = "HR")]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult MemberOvertimeViewByHR(DateTime? month, int? employeeId, string teamIds, string statusId)
        {
            if (!month.HasValue)
            {
                month = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            }

            var monthlyOvertimes = _OvertimeService.GetMemberOvertimeMonthly(this.CurrentUser.Id, "6", month.Value, employeeId, teamIds, statusId);

            ViewBag.CurrentUserId = this.CurrentUser.Id;
            return View(monthlyOvertimes);
        }

        /// <summary>
        /// Member overtime daily detail.
        /// </summary>
        /// <param name="requestId">Request Id.</param>
        /// <param name="isHRView">Is HR view or Verifier/Approver view.</param>
        /// <returns>Action Result.</returns>
        public ActionResult MemberDailyOvertime(int requestId, bool isHRView)
        {
            IList<OvertimeRequest> overtimeDaily = null;

            var overtimeMonthlyRequest = _OvertimeService.GetOvertimeRequestById(requestId);
            if (overtimeMonthlyRequest != null)
            {
                overtimeDaily = _OvertimeService.GetOvertimeRequests(overtimeMonthlyRequest.EmpId, overtimeMonthlyRequest.RequestedDate);
            }

            var overtimeDailyViewModel = new MemberOvertimeDailyViewModel
            {
                Month = overtimeMonthlyRequest.RequestedDate,
                DailyOvertimes = overtimeDaily,
                MonthlyOvertime = overtimeMonthlyRequest,
                CurrentUserId = this.CurrentUser.Id,
                IsHRView = isHRView
            };

            return View(overtimeDailyViewModel);
        }

        /// <summary>
        /// Action View Overtime Report Details.
        /// </summary>
        /// <returns>View OTExcelReport.</returns>
        public ActionResult OTExcelReport()
        {
            ViewBag.Groups = _TeamService.GetAll();
            var listJobTitle = _lookupDataService.GetAll(LookupCodeName.JobTitle);
            ViewBag.JobTitles = listJobTitle.Select(j => new SelectListItem
            {
                Text = j.Name,
                Value = j.Id.ToString()
            }).ToList();

            return View();
        }

        public ActionResult OTExcelReportDetails(DateTime fromDate, DateTime toDate, int? empId, string jobTitle, string groups, byte? isLocal)
        {
            var entity = _OvertimeService.OTExcelReportDetails(fromDate, toDate, empId, jobTitle, groups, isLocal);
            return PartialView(entity);
        }

        [HttpPost]
        [Authorize(Roles = "HR")]
        public ActionResult CheckInvalidOvertime(string requestIds)
        {
            var isInvalid = _OvertimeService.CheckInvalidOvertimes(requestIds);

            return Json(isInvalid, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize(Roles = "HR")]
        public void ReminderClaimOvertime(string subject, string content)
        {
            var sendMailTask = new Task(() => _OvertimeService.SendMailToReminderClaimOvertime(subject, content));
            sendMailTask.Start();
        }

        /// <summary>
        /// Gets the verifier approver list.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="viewModel">The view model.</param>
        [NonAction]
        private void GetVerifierApproverList(int id, MyOverTimeViewModel viewModel)
        {
            var verifierList = this.GetSupervisorByRequestType(id, (byte)RolesEnum.Verifier, (byte)RequestTypes.Overtime);
            var approverList = this.GetSupervisorByRequestType(id, (byte)RolesEnum.Approver, (byte)RequestTypes.Overtime);
            viewModel.Verifier = verifierList.Select(x => new SelectListItem { Text = x.NickName + " (" + x.FullName + ")", Value = x.OwnerId.ToString() }).ToList();
            viewModel.Approver = approverList.Select(x => new SelectListItem { Text = x.NickName + " (" + x.FullName + ")", Value = x.OwnerId.ToString() }).ToList();
        }

        /// <summary>
        /// Function to get member overtime view model.
        /// </summary>
        /// <returns>MemberOvertime ViewModel.</returns>
        [NonAction]
        private MemberOvertimeViewModel GetMemberOvertimeViewModel()
        {
            var month = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var teams = _TeamService.GetAll();
            var memberOvertimeViewModel = new MemberOvertimeViewModel
            {
                Teams = teams,
                Month = month
            };

            return memberOvertimeViewModel;
        }
    }
}
