namespace Fanex.NexHR.Controllers
{
    using System.Web.Mvc;
    using Core.Models;
    using Core.Services;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Filters;
    using Fanex.NexHR.Criteria.Unit;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels.Unit;

    public class UnitController : BaseController
    {
        private readonly IUnitService unitService;

        public UnitController(
            ISessionService sessionService,
            IUnitService unitService) : base(sessionService)
        {
            this.unitService = unitService;
        }

        [HRMPermission(Policy.Read, FunctionKey.UnitManagement)]
        public ActionResult Index(bool? isActive = null)
        {
            ViewBag.HasPermission = sessionService.HasPermissionOnFunction(FunctionKey.UnitManagement, Policy.Host);
            var model = new UnitViewModel
            {
                IsActive = isActive,
                Units = unitService.GetUnit(isActive: isActive)
            };

            return View(model);
        }

        [HttpPost]
        [HRMPermission(Policy.Read, FunctionKey.UnitManagement)]
        public ActionResult Index(UnitViewModel model)
        {
            if (model != null)
            {
                model.Units = unitService.GetUnit(isActive: model.IsActive);
            }

            return View(model);
        }

        [HRMPermission(Policy.Host, FunctionKey.UnitManagement)]
        public ActionResult Create()
        {
            return View(new AddUnitViewModel { });
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.UnitManagement)]
        public JsonResult Create(AddUnitViewModel model)
        {
            var commandResult = new CommandResult();
            if (model != null && ModelState.IsValid)
            {
                var criteria = new InsertUnitCriteria
                {
                    Code = model.Code,
                    Name = model.Name,
                    IsActive = model.IsActive,
                    CreatedBy = sessionService.Employee.Id
                };
                commandResult = unitService.Create(criteria);
            }

            return Json(commandResult);
        }

        [HRMPermission(Policy.Host, FunctionKey.UnitManagement)]
        public ActionResult Edit(int id)
        {
            Unit unit = unitService.GetById(id);
            if (unit == null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }

            var model = new EditUnitViewModel
            {
                UnitId = id,
                Code = unit.Code,
                Name = unit.Name,
                IsActive = unit.IsActive,
            };
            return View(model);
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.UnitManagement)]
        public JsonResult Edit(EditUnitViewModel model)
        {
            var commandResult = new CommandResult();
            if (model != null && ModelState.IsValid)
            {
                var criteria = new UpdateUnitCriteria
                {
                    UnitId = model.UnitId,
                    Code = model.Code,
                    Name = model.Name,
                    IsActive = model.IsActive,
                    ModifiedBy = sessionService.Employee.Id
                };
                commandResult = unitService.Edit(criteria);
            }

            return Json(commandResult);
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.UnitManagement)]
        public JsonResult Delete(DeleteUnitCriteria criteria)
        {
            var commandResult = unitService.Delete(criteria);

            return Json(commandResult);
        }
    }
}