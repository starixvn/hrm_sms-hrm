using System.Collections.Generic;

namespace Nexus.RoleManagement.Services
{
    public class MenuService : BaseService, IMenuService
    {
        private readonly IMenuRepository _menuRepository;

        public MenuService()
            : this(null)
        {
        }

        public MenuService(IMenuRepository menuRepository)
        {
            _menuRepository = menuRepository ?? new MenuRepository();
        }

        public IEnumerable<MenuModel> GetMenusByUserId(int userId)
        {
            var criteria = new GetMenusByUserIdCriteria { UserId = userId };
            IEnumerable<MenuModel> menus = new List<MenuModel>();
            if (criteria.IsValid())
            {
                menus = GetData<MenuModel>(criteria);
            }

            return menus;
        }

        public IEnumerable<MenuViewModel> GetMenus()
        {
            var criteria = new GetMenusCriteria();
            IEnumerable<MenuViewModel> menus = new List<MenuViewModel>();
            if (criteria.IsValid())
            {
                menus = _menuRepository.GetMenus(criteria);
            }

            return menus;
        }

        public Result Insert(InsertMenuCriteria criteria)
        {
            var result = new Result();
            if (criteria.IsValid())
            {
                result = Repository.Insert(criteria);
            }

            return result;
        }

        public MenuModel GetMenusById(int id)
        {
            var criteria = new GetMenuByIdCriteria { MenuId = id };
            var menu = new MenuModel();
            if (criteria.IsValid())
            {
                return GetDataEntity<MenuModel>(criteria);
            }

            return null;
        }

        public Result Update(UpdateMenuCriteria criteria)
        {
            var result = new Result();
            if (criteria.IsValid())
            {
                result = Repository.Update(criteria);
            }

            return result;
        }

        public Result Delete(int id)
        {
            var criteria = new DeleteMenuCriteria { MenuId = id };
            var result = new Result();
            if (criteria.IsValid())
            {
                result = Repository.Delete(criteria);
            }

            return result;
        }
    }
}
