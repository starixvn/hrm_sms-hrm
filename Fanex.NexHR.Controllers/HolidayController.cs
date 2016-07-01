namespace Fanex.NexHR.Controllers
{
    using Criteria.Holidays;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.Web.Providers;
    using Nexus.RoleManagement.Services;
    using System;
    using System.Web.Mvc;

    public class HolidayController : BaseController
    {
        private readonly IHolidayService holidayService;
        private readonly ICommandService commandService;

        public HolidayController(IHolidayService holidayService, ICommandService commandService)
        {
            this.holidayService = holidayService ?? new HolidayService();
            this.commandService = commandService ?? new CommandService();
        }

        public HolidayController()
            : this(null, null)
        {
        }

        [HttpGet]
        [HRAuthorize(Policy.Read, FunctionKey.Holiday)]
        public ActionResult Index(int? year)
        {
            var criteria = new GetHolidaysCriteria { Year = year ?? DateTime.Now.Year };

            ViewBag.Policies = new PoliciesModel
            {
                CanCreate = CurrentUser.HasPermission(FunctionKey.Holiday, Policy.Create),
                CanEdit = CurrentUser.HasPermission(FunctionKey.Holiday, Policy.Edit),
                CanDelete = CurrentUser.HasPermission(FunctionKey.Holiday, Policy.Delete)
            };
            ViewBag.Year = criteria.Year;

            return View(holidayService.GetByYear(criteria));
        }

        [HRAuthorize(Policy.Edit, FunctionKey.Holiday)]
        public ActionResult Edit(int id)
        {
            var criteria = new GetHolidayByIdCriteria { Id = id };

            return View(holidayService.GetById(criteria));
        }

        [HttpPost]
        [HRAuthorize(Policy.Edit, FunctionKey.Holiday)]
        public ActionResult Edit(Holiday model)
        {
            if (ModelState.IsValid)
            {
                var criteria = new UpdateHolidayCriteria
                {
                    Id = model.Id,
                    HolidayName = model.HolidayName,
                    FromDate = model.FromDate,
                    ToDate = model.ToDate,
                    ModifiedBy = CurrentUser.Id,
                    ModifiedDate = DateTime.Now
                };

                ViewBag.Updated = commandService.Update(criteria).IsSuccessful;
            }
            else
            {
                ModelState.AddModelError(string.Empty, Messages.InvalidInputData);
            }

            return View(model);
        }

        [HRAuthorize(Policy.Create, FunctionKey.Holiday)]
        public ActionResult Create()
        {
            return View(new Holiday { Name = string.Empty, FromDate = DateTime.Now, ToDate = DateTime.Now });
        }

        [HttpPost]
        [HRAuthorize(Policy.Create, FunctionKey.Holiday)]
        public ActionResult Create(Holiday model)
        {
            if (ModelState.IsValid)
            {
                var criteria = new InsertHolidayCriteria
                {
                    HolidayName = model.HolidayName,
                    FromDate = model.FromDate,
                    ToDate = model.ToDate,
                    ModifiedBy = CurrentUser.Id,
                    ModifiedDate = DateTime.Now
                };

                var result = commandService.Insert(criteria);
                if (result.IsSuccessful)
                {
                    ViewBag.Updated = true;
                }
                else
                {
                    ViewBag.Updated = false;
                    ModelState.AddModelError(string.Empty, "Duplicate holiday");
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, Messages.InvalidInputData);
            }

            return View(model);
        }

        [HttpPost]
        [HRAuthorize(Policy.Delete, FunctionKey.Holiday)]
        public bool Delete(int id)
        {
            var criteria = new DeleteHolidayCriteria { Id = id, ModifiedBy = CurrentUser.Id, ModifiedDate = DateTime.Now };

            return commandService.Delete(criteria).IsSuccessful;
        }
    }
}