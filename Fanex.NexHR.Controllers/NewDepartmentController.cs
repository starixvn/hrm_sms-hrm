namespace Fanex.NexHR.Controllers
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Core.Services;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Filters;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;

    public class NewDepartmentController : BaseController
    {
        private readonly ICommonDataService commonDataService;
        private readonly INewDepartmentService departmentService;
        private readonly IBaseService baseService;

        public NewDepartmentController(
            ISessionService sessionService,
            ICommonDataService commonDataService,
            INewDepartmentService departmentService,
            IBaseService baseService) : base(sessionService)
        {
            this.commonDataService = commonDataService;
            this.departmentService = departmentService;
            this.baseService = baseService;
        }

        [HRMPermission(Policy.Read, FunctionKey.DivisionManagement)]
        public ActionResult Index(bool? isActive = null)
        {
            ViewBag.HasPermission = sessionService.HasPermissionOnFunction(FunctionKey.DepartmentManagement, Policy.Host);
            IList<NewDepartment> departments = departmentService.GetAll(isActive);
            //return View(departments);
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }
    }
}