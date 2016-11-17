namespace Nexus.RoleManagement.Services
{
    using System.Collections.Generic;
    using System.Linq;

    public class MenuService : BaseService, IMenuService
    {
        private readonly IMenuRepository menuRepository;

        public MenuService()
            : this(null)
        {
        }

        public MenuService(IMenuRepository menuRepository)
        {
            this.menuRepository = menuRepository ?? new MenuRepository();
        }

        public IEnumerable<MenuModel> GetMenusByUserId(int userId)
        {
            var criteria = new GetMenusByUserIdCriteria { UserId = userId };
            IEnumerable<MenuModel> menus = new List<MenuModel>();
            if (criteria.IsValid())
            {
                menus = GetData<MenuModel>(criteria);
                menus = menus.GroupBy(i => i.Name).Select(g => g.First()).ToList();
            }

            return menus;
        }

        public IEnumerable<MenuViewModel> GetMenus()
        {
            var criteria = new GetMenusCriteria();
            IEnumerable<MenuViewModel> menus = new List<MenuViewModel>();
            if (criteria != null && criteria.IsValid())
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
                menu = GetDataEntity<MenuModel>(criteria);
            }

            return menu;
        }

        public Result Insert(InsertMenuCriteria criteria)
        {
            var result = new Result();
            if (criteria != null && criteria.IsValid())
            {
                result = Repository.Insert(criteria);
            }

            return result;
        }

        public Result Update(UpdateMenuCriteria criteria)
        {
            var result = new Result();
            if (criteria != null && criteria.IsValid())
            {
                result = Repository.Update(criteria);
            }

            return result;
        }

        public Result Delete(int id)
        {
            var criteria = new DeleteMenuCriteria { MenuId = id };
            var result = new Result();
            if (criteria != null && criteria.IsValid())
            {
                result = Repository.Delete(criteria);
            }

            return result;
        }
    }
}