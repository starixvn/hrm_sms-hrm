using System.Collections.Generic;

namespace Nexus.RoleManagement.Services
{
    public class MenuService : BaseService, IMenuService
    {
        public IEnumerable<MenuModel> GetMenusByUserId(int userId)
        {
            var criteria = new GetFunctionByUserIdCriteria { UserId = userId };
            IEnumerable<MenuModel> menus = new List<MenuModel>();
            if (criteria.IsValid())
            {
                menus = GetData<MenuModel>(criteria);
            }

            return menus;
        }
    }
}
