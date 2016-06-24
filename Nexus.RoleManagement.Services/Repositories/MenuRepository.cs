using Fanex.Data;
using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nexus.RoleManagement.Services
{
    public class MenuRepository : IMenuRepository
    {
        public IEnumerable<MenuViewModel> GetMenus(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var menus = db.Query<MenuViewModel, FunctionModel, MenuModel, MenuViewModel>(
                    (m, f, mP) =>
                    {
                        m.Function = f;
                        m.ParentMenu = mP;

                        return m;
                    },
                    splitOn: "FunctionId,MenuId",
                        parameterValues: criteria.GetSpParams());

                return menus;
            }
        }
    }
}
