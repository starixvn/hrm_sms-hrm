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
        private readonly IUnitService _unitService;
        private readonly IBaseService _baseService;

        public UnitController(
            ISessionService sessionService,
            IUnitService unitService,
            IBaseService baseService) : base(sessionService)
        {
            this._unitService = unitService;
            this._baseService = baseService;
        }

        [HRMPermission(Policy.Read, FunctionKey.UnitManagement)]
        public ActionResult Index(bool? isActive = null)
        {
            ViewBag.HasPermission = sessionService.HasPermissionOnFunction(FunctionKey.UnitManagement, Policy.Host);
            var model = new UnitViewModel
            {
                IsActive = isActive,
                Units = _unitService.GetUnit(isActive: isActive)
            };

            return View(model);
        }

        [HttpPost]
        [HRMPermission(Policy.Read, FunctionKey.UnitManagement)]
        public ActionResult Index(UnitViewModel model)
        {
            if (model != null)
            {
                model.Units = _unitService.GetUnit(isActive: model.IsActive);
            }

            ViewBag.HasPermission = sessionService.HasPermissionOnFunction(FunctionKey.UnitManagement, Policy.Host);

            return View(model);
        }

        [HRMPermission(Policy.Host, FunctionKey.UnitManagement)]
        public ActionResult Create()
        {
            var model = new AddUnitViewModel{};
        
            return View(model);
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.UnitManagement)]
        public JsonResult Create(InsertUnitCriteria criteria)
        {
            var commandResult = new CommandResult();

            criteria.CreatedBy = sessionService.Employee.Id;

            if (criteria.IsValid())
            {
                commandResult = _baseService.ExecuteNonQuery(criteria);
            }

            return Json(commandResult);
        }

        [HRMPermission(Policy.Host, FunctionKey.UnitManagement)]
        public ActionResult Edit(int unitId)
        {
            Unit unit = _unitService.GetById(unitId);
            
            var model = new EditUnitViewModel
            {
                UnitId = unitId,
                Code = unit.Code,
                Name = unit.Name,
                IsActive = unit.IsActive,
            };

            return View(model);
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.UnitManagement)]
        public JsonResult Edit(UpdateUnitCriteria criteria)
        {
            var commandResult = new CommandResult();
            
            criteria.ModifiedBy = sessionService.Employee.Id;

            if (criteria.IsValid())
            {
                commandResult = _baseService.ExecuteNonQuery(criteria);
            }

            return Json(commandResult);
        }
     
        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.UnitManagement)]
        public JsonResult Delete(DeleteUnitCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria.IsValid())
            {
                commandResult = _baseService.ExecuteNonQuery(criteria);
            }
            return Json(commandResult);
        }
    }
}