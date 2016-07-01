namespace Fanex.NexHR.Controllers
{
    using Criteria.Holidays;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.TimeRecord;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.Web.Providers;
    using Nexus.RoleManagement.Services;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    public class AjaxController : BaseController
    {
        private readonly IRequestService requestService;
        private readonly IEmployeeService employeeService;
        private readonly ILookupDataService lookupDataService;
        private readonly IReportService reportService;
        private readonly ICommandService commandService;

        public AjaxController()
            : this(null, null, null, null, null)
        {
        }

        public AjaxController(
            IRequestService requestService,
            IEmployeeService employeeService,
            ILookupDataService lookupDataService,
            IReportService reportService,
            ICommandService commandService)
        {
            this.requestService = requestService ?? new RequestService();
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

        /// [Authorize(Roles = "HR")]
        [HttpPost]
        [HRAuthorize(Policy.Edit, FunctionKey.TimeRecord)]
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
        [HRAuthorize(Policy.Delete, FunctionKey.Holiday)]
        public bool DeleteHoliday(int id)
        {
            var criteria = new DeleteHolidayCriteria { Id = id, ModifiedBy = CurrentUser.Id, ModifiedDate = DateTime.Now };

            return commandService.Delete(criteria).IsSuccessful;
        }
    }
}
