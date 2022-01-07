namespace HRM.UserControl.Client.Core.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using HRM.UserControl.Client.Core.Client;
    using HRM.UserControl.Client.Core.Models;

    public class FunctionService : IFunctionService
    {
        private readonly FunctionClient functionClient;

        public FunctionService()
        {
            functionClient = new FunctionClient();
        }

        public IList<Function> GetBySiteId(short siteId, bool? isActive = true)
        {
            IList<Function> functions = functionClient.GetBySiteId(siteId, isActive);
            return functions.OrderBy(x => x.DisplayOrder).ToList();
        }

        public async Task<IList<Function>> GetBySiteIdAsync(short siteId, bool? isActive = true)
        {
            IList<Function> functions = await functionClient.GetBySiteIdAsync(siteId, isActive);
            return functions.OrderBy(x => x.DisplayOrder).ToList();
        }

        public IList<Function> GetHierarchyBySiteId(short siteId, bool? isActive = true)
        {
            IList<Function> functions = functionClient.GetBySiteId(siteId, isActive);
            return GetHierarchyFunction(functions);
        }

        public async Task<IList<Function>> GetHierarchyBySiteIdAsync(short siteId, bool? isActive = true)
        {
            IList<Function> functions = await functionClient.GetBySiteIdAsync(siteId, isActive);
            return GetHierarchyFunction(functions);
        }

        public IList<FunctionPermission> GetPermissionBySiteAndRoleId(short siteId, short roleId)
            => functionClient.GetPermissionBySiteAndRoleId(siteId, roleId);

        public async Task<IList<FunctionPermission>> GetPermissionBySiteAndRoleIdAsync(short siteId, short roleId)
            => await functionClient.GetPermissionBySiteAndRoleIdAsync(siteId, roleId);

        private IList<Function> GetHierarchyFunction(IList<Function> functions)
        {
            IList<Function> functionTrees = functions.Where(func => !func.ParentId.HasValue)
                .OrderBy(x => x.DisplayOrder).ToList();
            foreach (Function function in functionTrees)
            {
                function.Level = 0;
                GetChildFunctions(function, functions);
            }

            return functionTrees;
        }

        private void GetChildFunctions(Function function, IList<Function> functions)
        {
            IList<Function> functionItems = functions.Where(x => x.ParentId == function.FunctionId)
                .OrderBy(x => x.DisplayOrder).ToList();
            if (functionItems.Any())
            {
                function.Childs = function.Childs ?? new List<Function>();

                foreach (Function item in functionItems)
                {
                    item.Level = (byte)(function.Level + 1);
                    function.Childs.Add(item);
                    GetChildFunctions(item, functions);
                }
            }
        }
    }
}