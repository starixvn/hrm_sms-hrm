using Nexus.RoleManagement.Services.Services.Implement;
using System.Collections.Generic;
using System.Linq;

namespace Nexus.RoleManagement.Services
{
    public class FunctionService : BaseService, IFunctionService
    {
        public IEnumerable<FunctionModel> GetFunctions()
        {
            var criteria = new GetFunctionsCriteria();
            IEnumerable<FunctionModel> functions = GetData<FunctionModel>(criteria);

            return functions;
        }

        public IEnumerable<FunctionViewModel> GetFunctionsByUserId(int userId)
        {
            var criteria = new GetFunctionByUserIdCriteria { UserId = userId };
            IEnumerable<FunctionViewModel> functions = new List<FunctionViewModel>();
            if (criteria.IsValid())
            {
                functions = GetData<FunctionViewModel>(criteria);
            }

            return functions;
        }

        public IEnumerable<FunctionViewModel> GetFunctionsByRoleId(int roleId)
        {
            var criteria = new GetFunctionsByRoleIdCriteria { RoleId = roleId };
            IEnumerable<FunctionViewModel> functions = new List<FunctionViewModel>();

            if (criteria.IsValid())
            {
                functions = GetData<FunctionViewModel>(criteria);
                functions = functions.OrderBy(i => i.FunctionKey);
            }

            return functions;
        }

        public Result AssignFunctionsToRole(AssignFunctionToRoleCriteria criteria)
        {
            var commandResult = new Result();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }
    }
}
