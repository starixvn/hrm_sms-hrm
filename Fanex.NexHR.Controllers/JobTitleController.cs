namespace Fanex.NexHR.Controllers
{
    using Core.Enums;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Criteria.Lookup;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.Web.Providers;
    using Nexus.RoleManagement.Services;
    using Resources;
    using System.Collections.Generic;
    using System.Web.Mvc;

    public class JobTitleController : BaseController
    {
        private readonly ILookupDataService lookupDataService;
        private readonly ICommandService commandService;

        public JobTitleController()
            : this(null, null)
        {
        }

        public JobTitleController(ILookupDataService lookupDataService, ICommandService commandService)
        {
            this.lookupDataService = lookupDataService ?? new LookupDataService();
            this.commandService = commandService ?? new CommandService();
        }

        [HRAuthorize(Policy.Read, FunctionKey.JobTitleManagement)]
        public ActionResult JobTitleManagement()
        {
            IList<SelectListItem> jobCategories = GetJobCategories();
            ViewBag.HasPermission = CurrentUser.HasPermission(FunctionKey.JobTitleManagement, Policy.Host);

            return View(jobCategories);
        }

        [HRAuthorize(Policy.Read, FunctionKey.JobTitleManagement)]
        public ActionResult GetJobTitles(string jobTitle)
        {
            var criteria = new GetJobTitleCriteria { JobTitle = jobTitle };
            IList<LookupData> jobTitles = lookupDataService.GetJobTitle(criteria);
            ViewBag.HasPermission = CurrentUser.HasPermission(FunctionKey.JobTitleManagement, Policy.Host);

            return View("~/Views/JobTitle/JobTitleTable.cshtml", jobTitles);
        }

        [HttpPost]
        [HRAuthorize(Policy.Host, FunctionKey.JobTitleManagement)]
        public JsonResult Delete(int id)
        {
            var criteria = new DeleteLookupDataCriteria
            {
                Id = id
            };
            CommandResult result = commandService.Delete(criteria);
            return Json(result);
        }

        [HttpPost]
        [HRAuthorize(Policy.Host, FunctionKey.JobTitleManagement)]
        public JsonResult Insert(string name, int categoryId)
        {
            var criteria = new InsertLookupDataCriteria
            {
                LookupCode = Constants.JobTitle,
                Name = name,
                CategoryId = categoryId.ToString()
            };
            CommandResult result = commandService.Insert(criteria);
            return Json(result);
        }

        [HttpPost]
        [HRAuthorize(Policy.Host, FunctionKey.JobTitleManagement)]
        public JsonResult Update(int id, string name, int categoryId)
        {
            var criteria = new UpdateLookupDataCriteria
            {
                Id = id,
                Name = name,
                CategoryId = categoryId.ToString()
            };
            CommandResult result = commandService.Update(criteria);
            return Json(result);
        }

        private static IList<SelectListItem> GetJobCategories()
        {
            return new List<SelectListItem>
                {
                    new SelectListItem { Text = Labels.Worker, Value = ((int)JobTitleCategory.Worker).ToString() },
                    new SelectListItem { Text = Labels.Staff, Value = ((int)JobTitleCategory.Staff).ToString() },
                    new SelectListItem { Text = Labels.Executive, Value = ((int)JobTitleCategory.Executive).ToString() },
                    new SelectListItem { Text = Labels.ExecutiveWithPosition, Value = ((int)JobTitleCategory.ExecutiveWithPosition).ToString() },
                    new SelectListItem { Text = Labels.Management, Value = ((int)JobTitleCategory.Management).ToString() }
                };
        }
    }
}
