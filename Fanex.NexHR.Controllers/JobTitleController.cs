namespace Fanex.NexHR.Controllers
{
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Criteria.Lookup;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.Web.Providers;
    using Nexus.RoleManagement.Services;
    using System.Collections;
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

        /// [Authorize(Roles = "HR")]
        [HRAuthorize(Policy.Read, FunctionKey.JobTitleManagement)]
        public ActionResult JobTitleManagement()
        {
            var getAllJobTitleCriteria = new GetAllLookupDataCriteria
            {
                LookupCode = Constants.JobTitle
            };
            ViewBag.JobTitles = lookupDataService.GetAll(getAllJobTitleCriteria);
            return View();
        }

        public JsonResult GetJobTitle(string query = "")
        {
            var criteria = new GetJobTitleCriteria
            {
                JobTitle = query
            };

            IList<LookupData> datas = lookupDataService.GetJobTitle(criteria);

            return Json(datas, JsonRequestBehavior.AllowGet);
        }

        /// [Authorize(Roles = "HR")]
        [HttpPost]
        [HRAuthorize(Policy.Delete, FunctionKey.JobTitleManagement)]
        public JsonResult Delete(int id)
        {
            var criteria = new DeleteLookupDataCriteria
            {
                Id = id
            };
            CommandResult result = commandService.Delete(criteria);
            return Json(result);
        }

        /// [Authorize(Roles = "HR")]
        [HttpPost]
        [HRAuthorize(Policy.Create, FunctionKey.JobTitleManagement)]
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

        /// [Authorize(Roles = "HR")]
        [HttpPost]
        [HRAuthorize(Policy.Edit, FunctionKey.JobTitleManagement)]
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
    }
}
