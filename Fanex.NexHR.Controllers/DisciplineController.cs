namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Filters;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Core.XMLExtensions;
    using Fanex.NexHR.Criteria.Discipline;
    using Fanex.NexHR.Models.Discipline;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels.Discipline;

    public class DisciplineController : BaseController
    {
        private readonly IDisciplineService disciplineService;
        private readonly IBaseService baseService;

        public DisciplineController(
            ISessionService sessionService,
            IDisciplineService disciplineService,
            IBaseService baseService) : base(sessionService)
        {
            this.disciplineService = disciplineService;
            this.baseService = baseService;
        }

        [HRMPermission(Policy.Read, FunctionKey.Discipline)]
        public ActionResult DisciplineSummary(DisciplineSummaryViewModel model)
        {
            if (model != null)
            {
                model.FromDate = GetFromDate();
                model.ToDate = GetToDate();
                model.DisciplineTypes = GetDisciplineType(true);
                model.DateRange = Utilities.GetDateRangeByQuarter();
            }

            ViewBag.IsHost = sessionService.HasPermissionOnFunction(FunctionKey.Discipline, Policy.Host);
            ViewBag.IsHostOtherDiscipline = sessionService.HasPermissionOnFunction(FunctionKey.OtherDiscipline, Policy.Host);
            return View(model);
        }

        [HRMPermission(Policy.Read, FunctionKey.Discipline)]
        public ActionResult GetDisciplineSummaryGrid(DisciplineSummaryViewModel model)
        {
            var criteria = new GetDisciplineSummaryCriteria
            {
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                GroupId = model.TeamId,
                IsLocal = model.IsLocal,
                EmployeeId = model.EmployeeId,
                ReasonId = model.ReasonId,
                ApproverId = sessionService.Employee.Id,
                IsShowAll = model.IsShowAll
            };

            IList<DisciplineSummaryModel> disciplines = disciplineService.GetDisciplineSummary(criteria);

            ViewBag.IsHost = sessionService.HasPermissionOnFunction(FunctionKey.Discipline, Policy.Host);

            return PartialView("_DisciplineSummaryGrid", disciplines);
        }

        [HRMPermission(Policy.Read, FunctionKey.Discipline)]
        public ActionResult DisciplineDetail(DisciplineSummaryViewModel model)
        {
            if (model != null)
            {
                if (model.FromDate == default(DateTime))
                {
                    model.FromDate = GetFromDate();
                    model.ToDate = GetToDate();
                }

                if (model.ReasonId < 0)
                {
                    model.ReasonId = null;
                }

                model.DisciplineTypes = GetDisciplineType(false);
                model.DateRange = Utilities.GetDateRangeByQuarter();
            }

            return View(model);
        }

        private static DateTime GetFromDate()
        {
            var today = DateTime.Today;
            int month = DateTime.Today.Month;
            DateTime fromDate = (month >= 1 && month <= 6) ? new DateTime(today.Year, 1, 1) : new DateTime(today.Year, 7, 1);
            return fromDate;
        }

        private static DateTime GetToDate()
        {
            var today = DateTime.Today;
            int month = DateTime.Today.Month;
            DateTime toDate = (month >= 1 && month <= 6)
                    ? new DateTime(today.Year, 6, DateTime.DaysInMonth(today.Year, 6))
                    : new DateTime(today.Year, 12, DateTime.DaysInMonth(today.Year, 12));
            return toDate;
        }

        [HRMPermission(Policy.Read, FunctionKey.Discipline)]
        public ActionResult GetDisciplineDetailGrid(DisciplineSummaryViewModel model)
        {
            var criteria = new GetDisciplineDetailCriteria
            {
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                GroupId = model.TeamId,
                IsLocal = model.IsLocal,
                EmployeeId = model.EmployeeId,
                ReasonId = model.ReasonId,
                HasIgnored = model.HasIgnored,
                ApproverId = sessionService.Employee.Id
            };

            IList<DisciplineDetailModel> disciplines = disciplineService.GetDisciplineDetail(criteria);
            ViewBag.HasPermissionToEdit = sessionService.HasPermissionOnFunction(FunctionKey.Discipline, Policy.Host);
            ViewBag.HasPermissionToUpdateOutOfControlReason = sessionService.HasPermissionOnFunction(FunctionKey.UpdateOutOfControlReason, Policy.Host);

            return PartialView("_DisciplineDetailGrid", disciplines);
        }

        [HRMPermission(Policy.Read, FunctionKey.Discipline)]
        public ActionResult ViewOtherDiscipline(int employeeId, DateTime fromDate, DateTime toDate)
        {
            var criteria = new GetOtherDisciplineOfEmployeeCriteria
            {
                EmployeeId = employeeId,
                FromDate = fromDate,
                ToDate = toDate
            };

            IList<OtherDisciplineModel> otherDisciplines = disciplineService.GetOtherDiscipline(criteria);
            IEnumerable<EmployeeInfoModel> members = disciplineService.GetMemberToAddOtherDiscipline(sessionService.Employee.Id);
            ViewBag.IsHost = members.Any(x => x.EmployeeId == employeeId)
                && sessionService.HasPermissionOnFunction(FunctionKey.Discipline, Policy.Host);

            return PartialView("_ViewOtherDiscipline", otherDisciplines);
        }

        [HRMPermission(Policy.Host, FunctionKey.OtherDiscipline)]
        public ActionResult PartialAddOtherDiscipline(string employee, int? employeeId = null)
        {
            var criteria = new GetOtherDisciplineTypeCriteria();
            IList<DisciplineTypeModel> otherDisciplineTypes = disciplineService.GetOtherDisciplineType(criteria);

            var model = new AddOtherDisciplineViewModel
            {
                EmployeeId = employeeId ?? -1,
                Employee = employee,
                WorkDate = DateTime.Now,
                OtherDisciplineTypes = otherDisciplineTypes
            };

            return PartialView("_AddOtherDiscipline", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        [HRMPermission(Policy.Host, FunctionKey.OtherDiscipline)]
        public JsonResult AddOtherDiscipline(AddOtherDisciplineViewModel model)
        {
            var criteria = new InsertOtherDisciplineCriteria
            {
                EmployeeId = model.EmployeeId,
                WorkDate = model.WorkDate,
                DisciplineTypeId = model.DisciplineTypeId,
                Score = model.MinusScore,
                Comment = model.Comment,
                ActorId = sessionService.Employee.Id
            };
            CommandResult commandResult = baseService.ExecuteNonQuery(criteria);

            return Json(commandResult);
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.OtherDiscipline)]
        public string DeleteOtherDiscipline(DeleteOtherDisciplineCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ActorId = sessionService.Employee.Id;
                CommandResult commandResult = baseService.ExecuteNonQuery(criteria);

                if (!commandResult.IsSuccessful)
                {
                    return commandResult.Message;
                }
            }

            return string.Empty;
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.Discipline)]
        public CustomJsonResult EditDisciplineIgnore(IEnumerable<IgnoredDisciplineModel> ignoreDetails)
        {
            var criteria = new UpdateIgnoredDisciplineCriteria
            {
                IgnoreDetailXml = ignoreDetails.ToXML().ToString(),
                HasUpdateOutOfControlReason = sessionService.HasPermissionOnFunction(FunctionKey.UpdateOutOfControlReason, Policy.Host),
                ActorId = sessionService.Employee.Id,
            };
            CommandResult commandResult = baseService.ExecuteNonQuery(criteria);

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult GetMemberToAddOtherDiscipline(string keyword)
        {
            IEnumerable<EmployeeInfoModel> employees = disciplineService.GetMemberToAddOtherDiscipline(sessionService.Employee.Id, keyword);

            return Json(employees, JsonRequestBehavior.AllowGet);
        }

        private IList<DisciplineTypeModel> GetDisciplineType(bool includeOtherType)
        {
            var criteria = new GetDisciplineTypeCriteria
            {
                IncludeOtherType = includeOtherType
            };
            IList<DisciplineTypeModel> disciplineTypes = disciplineService.GetDisciplineType(criteria);

            return disciplineTypes;
        }
    }
}