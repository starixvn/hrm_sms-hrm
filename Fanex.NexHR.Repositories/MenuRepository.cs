namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.ViewModels;

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