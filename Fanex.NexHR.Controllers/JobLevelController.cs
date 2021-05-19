namespace Fanex.NexHR.Controllers
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Core.Enums;
    using Core.Services;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Filters;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Criteria.JobLevel;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services.Interfaces;

    public class JobLevelController : BaseController
    {
        private readonly IJobLevelService jobLevelService;

        public JobLevelController(
            ISessionService sessionService,
            IJobLevelService jobLevelService) : base(sessionService)
        {
            this.jobLevelService = jobLevelService;
        }

        [HRMPermission(Policy.Read, FunctionKey.JobLevelManagement)]
        public ActionResult Index()
        {
            ViewBag.HasPermission = sessionService.HasPermissionOnFunction(FunctionKey.JobLevelManagement, Policy.Host);
            return View();
        }

        [ValidateInput(false)]
        [HRMPermission(Policy.Read, FunctionKey.JobLevelManagement)]
        public ActionResult GetJobLevel()
        {
            ViewBag.HasPermission = sessionService.HasPermissionOnFunction(FunctionKey.JobLevelManagement, Policy.Host);

            IList<JobLevel> jobLevel = jobLevelService.GetJobLevel();

            return PartialView("~/Views/JobLevel/_JobLevelGrid.cshtml", jobLevel);
        }

        [ValidateInput(false)]
        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.JobLevelManagement)]
        public JsonResult Insert(string name)
        {
            var criteria = new InsertJobLevelCriteria
            {
                Name = name,
                ModifiedBy = sessionService.Employee.Id
            };

            CommandResult result = jobLevelService.Insert(criteria);

            return Json(result);
        }

        [ValidateInput(false)]
        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.JobLevelManagement)]
        public JsonResult Update(int id, string name)
        {
            var criteria = new UpdateJobLevelCriteria
            {
                JobLevelId = id,
                Name = name,
                ModifiedBy = sessionService.Employee.Id
            };
            CommandResult result = jobLevelService.Update(criteria);

            return Json(result);
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.JobLevelManagement)]
        public JsonResult Delete(int id)
        {
            CommandResult result = jobLevelService.Delete(id, sessionService.Employee.Id);

            return Json(result);
        }
    }
}
