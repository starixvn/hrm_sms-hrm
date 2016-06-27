namespace Fanex.NexHR.Controllers
{
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.Web.Providers;
    using Nexus.RoleManagement.Services;
    using System;
    using System.Web.Mvc;

    public class HolidayController : BaseController
    {
        private readonly IHolidayService holidayService;

        public HolidayController(IHolidayService holidayService)
        {
            this.holidayService = holidayService ?? new HolidayService();
        }

        public HolidayController()
            : this(null)
        {
        }

        [HRAuthorize(Policy.Read, FunctionKey.Holiday)]
        public ActionResult Index()
        {
            return View(holidayService.GetByYear(DateTime.Now.Year));
        }

        [HttpGet]
        [HRAuthorize(Policy.Read, FunctionKey.Holiday)]
        public ActionResult Index(int? year)
        {
            ViewBag.year = year ?? DateTime.Now.Year;
            ViewBag.HasPermission = CurrentUser.HasPermission(FunctionKey.Holiday, Policy.Edit)
                || CurrentUser.HasPermission(FunctionKey.Holiday, Policy.Create)
                || CurrentUser.HasPermission(FunctionKey.Holiday, Policy.Delete);

            return View(holidayService.GetByYear(ViewBag.year));
        }

        /// [Authorize(Roles = "HR")]
        [HRAuthorize(Policy.Edit, FunctionKey.Holiday)]
        public ActionResult Edit(int id)
        {
            return View(holidayService.GetById(id));
        }

        /// <summary>
        /// ActionResult Edit.
        /// </summary>
        /// <param name="model">Holiday model.</param>
        /// <returns>View Edit.</returns>
        /// [Authorize(Roles = "HR")]
        [HttpPost]
        [HRAuthorize(Policy.Edit, FunctionKey.Holiday)]
        public ActionResult Edit(Holiday model)
        {
            if (ModelState.IsValid)
            {
                model.ModifiedDate = DateTime.Now;
                model.ModifiedBy = this.CurrentUser.Id;
                ViewBag.Updated = holidayService.Update(model);
            }
            else
            {
                ModelState.AddModelError(string.Empty, Resources.Messages.InvalidInputData);
            }

            return View(model);
        }

        /// [Authorize(Roles = "HR")]
        [HRAuthorize(Policy.Create, FunctionKey.Holiday)]
        public ActionResult Create()
        {
            return View(new Holiday
            {
                Name = string.Empty,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                ModifiedBy = this.CurrentUser.Id
            });
        }

        /// <summary>
        /// ActionResult Create.
        /// </summary>
        /// <param name="model">Holiday model.</param>
        /// <returns>View Create.</returns>
        /// [Authorize(Roles = "HR")]
        [HttpPost]
        [HRAuthorize(Policy.Create, FunctionKey.Holiday)]
        public ActionResult Create(Holiday model)
        {
            if (ModelState.IsValid)
            {
                model.ModifiedDate = DateTime.Now;
                model.ModifiedBy = this.CurrentUser.Id;
                var id = holidayService.Insert(model);
                if (id == -1)
                {
                    ViewBag.Updated = false;
                    ModelState.AddModelError(string.Empty, "Duplicate holiday");
                }
                else
                {
                    ViewBag.Updated = true;
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, Resources.Messages.InvalidInputData);
            }

            return View(model);
        }
    }
}