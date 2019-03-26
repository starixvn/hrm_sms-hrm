namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Linq;
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
        private readonly ICommandService commandService;

        public MenuService(
            IMenuRepository menuRepository,
            IBaseService baseService,
            ICommandService commandService)
        {
            this.menuRepository = menuRepository;
            this.commandService = commandService;
            this.baseService = baseService;
        }

        public IEnumerable<MenuModel> GetMenusByUserId(int userId)
        {
            var criteria = new GetMenusByUserIdCriteria { UserId = userId };
            IEnumerable<MenuModel> menus = new List<MenuModel>();
            if (criteria.IsValid())
            {
                menus = baseService.GetData<MenuModel>(criteria);
                menus = menus.GroupBy(i => i.Name).Select(g => g.First()).ToList();
            }

            return menus;
        }

        public IEnumerable<MenuViewModel> GetMenus()
        {
            var criteria = new GetMenusCriteria();

            IEnumerable<MenuViewModel> menus = new List<MenuViewModel>();

            if (criteria.IsValid())
            {
                menus = menuRepository.GetMenus(criteria);
            }

            return menus;
        }

        public MenuModel GetMenusById(int id)
        {
            var criteria = new GetMenuByIdCriteria { MenuId = id };
            var menu = new MenuModel();
            if (criteria.IsValid())
            {
                menu = baseService.GetDataEntity<MenuModel>(criteria);
            }

            return menu;
        }

        public CommandResult Insert(InsertMenuCriteria criteria)
        {
            var result = new CommandResult();
            if (criteria != null && criteria.IsValid())
            {
                result = commandService.Insert(criteria);
            }

            return result;
        }

        public CommandResult Update(UpdateMenuCriteria criteria)
        {
            var result = new CommandResult();
            if (criteria != null && criteria.IsValid())
            {
                result = commandService.Update(criteria);
            }

            return result;
        }

        public CommandResult Delete(int id)
        {
            var criteria = new DeleteMenuCriteria { MenuId = id };
            var result = new CommandResult();
            if (criteria.IsValid())
            {
                result = commandService.Delete(criteria);
            }

            return result;
        }
    }
}