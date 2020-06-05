namespace Fanex.NexHR.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Core.Models;
    using Core.Services;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Filters;
    using Fanex.NexHR.Criteria.Section;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels;
    using Fanex.NexHR.ViewModels.Section;

    public class SectionController : BaseController
    {
        private readonly ICommonDataService commonDataService;
        private readonly ISectionService sectionService;
        private readonly IBaseService baseService;

        public SectionController(
            ISessionService sessionService,
            ICommonDataService commonDataService,
            ISectionService sectionService,
            IBaseService baseService) : base(sessionService)
        {
            this.commonDataService = commonDataService;
            this.sectionService = sectionService;
            this.baseService = baseService;
        }

        [HRMPermission(Policy.Read, FunctionKey.SectionManagement)]
        public ActionResult Index(bool? isActive = null)
        {
            ViewBag.HasPermission = sessionService.HasPermissionOnFunction(FunctionKey.SectionManagement, Policy.Host);
            var model = new SectionViewModel
            {
                IsActive = isActive,
                Sections = sectionService.GetSection(isActive: isActive)
            };

            return View(model);
        }

        [HttpPost]
        [HRMPermission(Policy.Read, FunctionKey.SectionManagement)]
        public ActionResult Index(SectionViewModel model)
        {
            if (model != null)
            {
                model.Sections = sectionService.GetSection(isActive: model.IsActive);
            }

            ViewBag.HasPermission = sessionService.HasPermissionOnFunction(FunctionKey.SectionManagement, Policy.Host);

            return View(model);
        }

        [HRMPermission(Policy.Host, FunctionKey.SectionManagement)]
        public ActionResult Create()
        {
            var model = new AddSectionViewModel
            {
                ParentSections = sectionService.GetSection()
            };
            return View(model);
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.SectionManagement)]
        public JsonResult Create(InsertSectionCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria.IsValid())
            {
                commandResult = baseService.ExecuteNonQuery(criteria);
            }

            return Json(commandResult);
        }

        [HRMPermission(Policy.Host, FunctionKey.SectionManagement)]
        public ActionResult Edit(int sectionId)
        {
            Section section = sectionService.GetById(sectionId);
            IList<Section> parentSections = sectionService.GetSection();
            parentSections = parentSections.Where(t => t.Id != section.Id).ToList();

            var model = new EditSectionViewModel
            {
                SectionId = sectionId,
                Code = section.Code,
                Name = section.Name,
                IsActive = section.IsActive,
                ParentSections = parentSections
            };
            return View(model);
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.SectionManagement)]
        public JsonResult Edit(UpdateSectionCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria.IsValid())
            {
                commandResult = baseService.ExecuteNonQuery(criteria);
            }

            return Json(commandResult);
        }

        public CustomJsonResult GetHierarchy()
        {
            IList<Team> teams = commonDataService.GetAllTeam();
            IEnumerable<MultiSelectViewModel> sections = sectionService.GetSection().Select(x => new MultiSelectViewModel
            {
                Id = x.Id,
                Text = x.Name,
                Items = teams.Where(y => y.Section != null && y.Section.Id == x.Id).Select(y => new MultiSelectViewModel
                {
                    Id = y.Id,
                    Text = y.Name,
                    IsLeaf = true
                }).ToList()
            }).ToList();
            IEnumerable<MultiSelectViewModel> hierarchy = GetHierarchy(sections);

            return new CustomJsonResult { Data = hierarchy, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        private IEnumerable<MultiSelectViewModel> GetHierarchy(IEnumerable<MultiSelectViewModel> sections)
        {
            IEnumerable<MultiSelectViewModel> parents = sections.Where(x => !x.ParentId.HasValue);
            foreach (MultiSelectViewModel parent in parents)
            {
                GetChilds(parent, sections);
            }

            return parents;
        }

        private void GetChilds(MultiSelectViewModel section, IEnumerable<MultiSelectViewModel> sections)
        {
            IEnumerable<MultiSelectViewModel> items = sections.Where(x => x.ParentId.HasValue && x.ParentId.Value == section.Id);

            if (items.Any())
            {
                foreach (MultiSelectViewModel item in items)
                {
                    section.Items.Add(item);
                    GetChilds(item, sections);
                }
            }
        }
    }
}