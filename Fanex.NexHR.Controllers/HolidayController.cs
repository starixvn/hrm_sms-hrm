namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Web.Mvc;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.Web.Providers;
    using Nexus.RoleManagement.Services;

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

        public ActionResult Index()
        {
            return View(holidayService.GetByYear(DateTime.Now.Year));
        }

        [HttpGet]
        public ActionResult Index(int? year)
        {
            ViewBag.year = year ?? DateTime.Now.Year;
            return View(holidayService.GetByYear(ViewBag.year));
        }

        [Authorize(Roles = "HR")]
        [HRAuthorize(FunctionKey = FunctionKey.Holiday, Policy = Policy.Edit)]
        public ActionResult Edit(int id)
        {
            return View(holidayService.GetById(id));
        }

        /// <summary>
        /// ActionResult Edit.
        /// </summary>
        /// <param name="model">Holiday model.</param>
        /// <returns>View Edit.</returns>
        [Authorize(Roles = "HR")]
        [HttpPost]
        [HRAuthorize(FunctionKey = FunctionKey.Holiday, Policy = Policy.Edit)]
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

        [Authorize(Roles = "HR")]
        [HRAuthorize(FunctionKey = FunctionKey.Holiday, Policy = Policy.Create)]
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
        [Authorize(Roles = "HR")]
        [HttpPost]
        [HRAuthorize(FunctionKey = FunctionKey.Holiday, Policy = Policy.Create)]
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