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

        public MenuService(
            IMenuRepository menuRepository,
            IBaseService baseService)
        {
            this.menuRepository = menuRepository;
            this.baseService = baseService;
        }

        public IEnumerable<MenuModel> GetMenusByUserId(int userId)
        {
            var criteria = new GetMenusByUserIdCriteria { UserId = userId };
            IEnumerable<MenuModel> menus = baseService.GetData<MenuModel>(criteria);
            menus = menus.GroupBy(i => i.Name).Select(g => g.First());

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
            return baseService.GetDataEntity<MenuModel>(criteria);
        }

        public CommandResult Insert(InsertMenuCriteria criteria)
        {
            return baseService.ExecuteNonQuery(criteria);
        }

        public CommandResult Update(UpdateMenuCriteria criteria)
        {
            return baseService.ExecuteNonQuery(criteria);
        }

        public CommandResult Delete(int id)
        {
            var criteria = new DeleteMenuCriteria { MenuId = id };

            return baseService.ExecuteNonQuery(criteria);
        }
    }
}