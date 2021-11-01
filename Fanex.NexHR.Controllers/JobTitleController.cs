namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Core.Enums;
    using Core.Models;
    using Core.Services;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Filters;
    using Fanex.NexHR.Criteria.Lookup;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using HRM.UserControl.Client.Core.Enumerables;
    using HRM.UserControl.Client.Core.Session;
    using Resources;

    public class JobTitleController : BaseExportController
    {
        private readonly IUserControlSession userControlSession;
        private readonly ICommonDataService commonDataService;
        private readonly IJobTitleExportService jobTitleExportService;
        private readonly IBaseService baseService;

        public JobTitleController(
            ISessionService sessionService,
            IUserControlSession userControlSession,
            ICommonDataService commonDataService,
            IJobTitleExportService jobTitleExportService,
            IBaseService baseService) : base(sessionService)
        {
            this.userControlSession = userControlSession;
            this.commonDataService = commonDataService;
            this.jobTitleExportService = jobTitleExportService;
            this.baseService = baseService;
        }

        [HRMPermission(Policy.Read, FunctionKey.JobTitleManagement)]
        public ActionResult JobTitleManagement()
        {
            IList<SelectListItem> jobCategories = GetJobCategories();
            ViewBag.HasPermission = userControlSession.HasPermissionOnFunction(FunctionKey.JobTitleManagement, Policy.Host);

            return View(jobCategories);
        }

        [ValidateInput(false)]
        [HRMPermission(Policy.Read, FunctionKey.JobTitleManagement)]
        public ActionResult GetJobTitles(string jobTitle)
        {
            IList<LookupData> jobTitles = commonDataService.GetJobTitle(jobTitle);
            ViewBag.HasPermission = userControlSession.HasPermissionOnFunction(FunctionKey.JobTitleManagement, Policy.Host);

            return PartialView("~/Views/JobTitle/_JobTitleGrid.cshtml", jobTitles);
        }

        [HRMPermission(Policy.Host, FunctionKey.JobTitleManagement)]
        public ActionResult ExportExcel(string jobTitle)
        {
            IList<LookupData> jobTitles = commonDataService.GetJobTitle(jobTitle);
            byte[] bytes = jobTitleExportService.Export(jobTitles);

            return File(bytes, MimeType.XLSX, $"JobTitle_{DateTime.Now.ToString("yyyyMMdd")}.xlsx");
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.JobTitleManagement)]
        public JsonResult Delete(int id)
        {
            var criteria = new DeleteLookupDataCriteria
            {
                Id = id
            };
            CommandResult result = baseService.ExecuteNonQuery(criteria);

            return Json(result);
        }

        [ValidateInput(false)]
        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.JobTitleManagement)]
        public JsonResult Insert(string name, int categoryId)
        {
            var criteria = new InsertLookupDataCriteria
            {
                LookupCode = Constants.JobTitle,
                Name = name,
                CategoryId = categoryId.ToString()
            };
            CommandResult result = baseService.ExecuteNonQuery(criteria);

            return Json(result);
        }

        [ValidateInput(false)]
        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.JobTitleManagement)]
        public JsonResult Update(int id, string name, int categoryId)
        {
            var criteria = new UpdateLookupDataCriteria
            {
                Id = id,
                Name = name,
                CategoryId = categoryId.ToString()
            };
            CommandResult result = baseService.ExecuteNonQuery(criteria);

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