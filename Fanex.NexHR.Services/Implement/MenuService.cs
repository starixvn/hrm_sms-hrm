namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Criteria.Menu;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.ViewModels;

    public class MenuService : IMenuService
    {
        private readonly IMenuRepository menuRepository;
        private readonly IBaseService baseService;

        public MenuService(
            IMenuRepository menuRepository,
            IBaseService baseService)
        {
            this.menuRepository = menuRepository;
            this.baseService = baseService;
        }

        public IEnumerable<MenuViewModel> GetMenus()
        {
            var criteria = new GetMenusCriteria();
            IEnumerable<MenuViewModel> menus = menuRepository.GetMenus(criteria);

            return menus ?? new List<MenuViewModel>();
        }

        public MenuModel GetMenusById(int id)
        {
            var criteria = new GetMenuByIdCriteria { MenuId = id };
            return baseService.GetDataEntity<MenuModel>(criteria);
        }

        public CommandResult Create(MenuModel model)
        {
            var criteria = new InsertMenuCriteria
            {
                DisplayOrder = model.DisplayOrder,
                FunctionId = model.FunctionId == 0 ? null : model.FunctionId,
                Link = model.Link,
                Name = model.Name,
                ParentId = model.ParentId == 0 ? null : model.ParentId,
                Text = model.Text
            };
            return baseService.ExecuteNonQuery(criteria);
        }

        public CommandResult Edit(MenuModel model)
        {
            var criteria = new UpdateMenuCriteria
            {
                DisplayOrder = model.DisplayOrder,
                FunctionId = model.FunctionId == 0 ? null : model.FunctionId,
                Link = model.Link,
                Name = model.Name,
                ParentId = model.ParentId == 0 ? null : model.ParentId,
                Text = model.Text,
                MenuId = model.MenuId,
                IsActive = model.IsActive
            };
            return baseService.ExecuteNonQuery(criteria);
        }

        public CommandResult Delete(int id)
        {
            var criteria = new DeleteMenuCriteria { MenuId = id };

            return baseService.ExecuteNonQuery(criteria);
        }
    }
}