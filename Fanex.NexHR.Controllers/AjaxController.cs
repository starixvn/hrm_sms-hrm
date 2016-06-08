namespace Fanex.NexHR.Controllers
{
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Criteria.AnnualLeave;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.Memo;
    using Fanex.NexHR.Criteria.Overtime;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Criteria.TimeRecord;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.AnnualLeave;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web.Mvc;

    public class AjaxController : BaseController
    {
        private readonly IRequestService requestService;
        private readonly IHolidayService holidayService;
        private readonly IEmployeeService employeeService;
        private readonly ILookupDataService lookupDataService;
        private readonly IReportService reportService;
        private readonly ICommandService commandService;

        public AjaxController()
            : this(null, null, null, null, null, null)
        {
        }

        public AjaxController(
            IRequestService requestService,
            IHolidayService holidayService,
            IEmployeeService employeeService,
            ILookupDataService lookupDataService,
            IReportService reportService,
            ICommandService commandService)
        {
            this.requestService = requestService ?? new RequestService();
            this.holidayService = holidayService ?? new HolidayService();
            this.employeeService = employeeService ?? new EmployeeService();
            this.lookupDataService = lookupDataService ?? new LookupDataService();
            this.reportService = reportService ?? new ReportService();
            this.commandService = commandService ?? new CommandService();
        }

        [HttpPost]
        public string GetMessageDetailRequest(RequestEntity request, short requestType, string fromShiftCode, string toShiftCode)
        {
            request.RequestType = new RequestTypeModel { Id = requestType };
            request.FromShift = new Shift { Name = fromShiftCode };
            request.ToShift = new Shift { Name = toShiftCode };

            var date = RequestService.ToRequestMessage(request);
            return date;
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public bool DeleteEmployee(int id, DateTime resignedDate)
        {
            var deleteEmployeeCriteria = new DeleteEmployeeCriteria
            {
                EmpId = id,
                ResignedDate = resignedDate
            };
            CommandResult result = employeeService.Delete(deleteEmployeeCriteria);

            return result.Code == 0;
        }

        [Authorize(Roles = "Verifier,Approver,HR")]
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult ProcessRequest(string command, long requestId, int roleId, string remark = null)
        {
            var criteria = new ProcessRequestCriteria()
            {
                Action = command,
                RequestId = requestId,
                RoleId = roleId,
                Comment = remark,
                ProcessedBy = CurrentUser.Id
            };
            CommandResult result = requestService.Process(criteria);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Verifier,Approver,HR")]
        [HttpPost]
        public JsonResult ProcessSingleOvertime(string command, long requestId, int roleId)
        {
            var processOvertimeCriteria = new ProcessSingleOvertimeCriteria
            {
                CommandProcess = command,
                RequestID = requestId,
                RoleID = roleId,
                ProcessedBy = this.CurrentUser.Id,
                ProcessedDate = DateTime.Now
            };
            CommandResult commandResult = requestService.ProcessOvertime(processOvertimeCriteria);
            return commandResult.Code == 0 ? Json(true) : Json(commandResult.Message);
        }

        [Authorize(Roles = "Verifier,Approver,HR")]
        [HttpPost]
        public JsonResult ProcessMultipleOvertime(string command, string requestIds)
        {
            var criteria = new ProcessMultipleOvertimeCriteria
            {
                CommandProcess = command,
                ListRequestID = requestIds,
                ProcessedBy = this.CurrentUser.Id,
                ProcessedDate = DateTime.Now
            };
            criteria.RoleID = criteria.GetRoleIdForProcessOvertime(command);
            CommandResult commandResult = requestService.ProcessOvertime(criteria);
            return commandResult.Code == 0 ? Json(true) : Json(commandResult.Message);
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public JsonResult ProcessInOut(TimeRecordUpdateTypeCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = CurrentUser.Id;
                criteria.ModifiedDate = DateTime.Now;
            }

            CommandResult commandResult = commandService.Update(criteria);
            return Json(commandResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMemberByManager(string keyword)
        {
            var criteria = new GetMemberByManagerCriteria()
            {
                SupervisorId = this.CurrentUser.Id,
                Query = keyword,
                ExcludedManager = false
            };
            IList<Employee> employees = employeeService.GetMemberByManager(criteria);

            return Json(employees.Take(10), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AdjustAnnualLeaves(string annualLeaveJson)
        {
            var model = JsonConverter.FromJson<AnnualLeaveBalanceMultiViewModel>(annualLeaveJson);
            var adjustAnnualLeaveCriteria = new AdjustEmployeeAnnualLeaveCriteria
            {
                EmpIds = model.EmpIds,
                Description = model.Description,
                CreatedBy = this.CurrentUser.Id,
                CreatedDate = DateTime.Now,
                CreditDays = model.CreditDays,
                DeductDays = model.DeductDays,
                FromDate = model.AffectedFrom
            };
            CommandResult result = employeeService.AdjustAnnualLeaves(adjustAnnualLeaveCriteria);
            if (result.Code != 0)
            {
                return Json(new { success = false, errors = result.Message });
            }

            return Json(true);
        }

        [HttpPost]
        public JsonResult ResetAnnualLeaves(int minbalance, string descriptionReset)
        {
            if (ModelState.IsValid)
            {
                int lastYear = DateTime.Now.AddYears(-1).Year;
                DateTime date = new DateTime(lastYear, 12, 31);

                var criteria = new GetAnnualLeaveBalanceCriteria()
                {
                    Year = lastYear,
                    EmpId = null,
                    GroupIds = null,
                    IsLocal = null,
                    Role = string.Empty,
                    SupervisorID = null
                };
                IList<LeaveSummary> entityList = reportService.GetAnnualLeaveBalanceReport(criteria);

                entityList = entityList.Where(m => m.LeaveBalance > minbalance).ToList();

                string template = "<Employee Id=\"{0}\" LeaveBalance=\"{1}\"/>";
                StringBuilder strValue = new StringBuilder();
                strValue.Append("<ROOT>");
                foreach (var item in entityList)
                {
                    strValue.Append(string.Format(template, item.Employee.Id, item.LeaveBalance));
                }

                strValue.Append("</ROOT>");

                var resetAnnualLeaveCriteria = new ResetAnnualLeaveCriteria
                {
                    CreatedBy = this.CurrentUser.Id,
                    CreatedDate = DateTime.Now,
                    Description = descriptionReset,
                    EmpValue = strValue.ToString(),
                    FromDate = date,
                    ResetDays = minbalance
                };
                CommandResult result = employeeService.ResetAnnualLeaves(resetAnnualLeaveCriteria);

                return Json(result.Code == 0);
            }
            else
            {
                return Json(-1);
            }
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public bool DeleteHoliday(int id)
        {
            return holidayService.Delete(id, this.CurrentUser.Id);
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public int ResetPassword(int id)
        {
            CommandResult result = employeeService.ResetPassword(id, "123456", CurrentUser.Id);
            return result.Code;
        }

        public int UpdatePasswordResetStatus(bool status)
        {
            var criteria = new UpdatePasswordResetStatusCriteria()
            {
                EmpId = this.CurrentUser.Id,
                PasswordReset = status
            };
            CommandResult result = employeeService.UpdatePasswordResetStatus(criteria);

            return result.Code;
        }
    }
}
