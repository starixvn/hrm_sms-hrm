namespace Fanex.NexHR.Controllers.ApiControllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Http;
    using Fanex.NexHR.Criteria.Report;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels;

    public class ReportController : ApiController
    {
        private readonly IReportService _reportService;

        public ReportController()
        {
            _reportService = new ReportService();
        }

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        public IList<EmployeeAttendanceModel> GetEmployeeAttendances([FromUri]EmployeeAttendanceViewModel model)
        {
            var criteria = new GetEmployeeAttendanceCriteria()
            {
                SupervisorId = model.SupervisorId,
                Fromtime = model.FromDate,
                Totime = model.ToDate,
                AttendanceType = model.AttendanceType,
                EmpId = model.EmpId,
                Groupids = model.GroupIds,
                ExcludeUserDelete = model.ExcludeDeletedUsers ? 1 : 0,
                IsLocal = model.IsLocal
            };
            var employeeAttendance = _reportService.GetEmployeeAttendanceReport(criteria);

            return employeeAttendance;
        }
    }
}