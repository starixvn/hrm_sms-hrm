namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Criteria.Function;
    using Fanex.NexHR.Models;

    public class FunctionService : IFunctionService
    {
        private readonly IBaseService baseService;
        private readonly ICommandService commandService;

        public FunctionService(
            IBaseService baseService,
            ICommandService commandService)
        {
            this.commandService = commandService;
            this.baseService = baseService;
        }

        public IEnumerable<FunctionModel> GetFunctions()
        {
            var criteria = new GetFunctionsCriteria();
            return baseService.GetData<FunctionModel>(criteria);
        }

        public IEnumerable<FunctionModel> GetFunctionsByUserId(int userId)
        {
            var criteria = new GetFunctionByUserIdCriteria { UserId = userId };
            IEnumerable<FunctionModel> functions = new List<FunctionModel>();
            if (criteria.IsValid())
            {
                functions = baseService.GetData<FunctionModel>(criteria);
            }

            return functions;
        }

        public IEnumerable<FunctionModel> GetFunctionsByRoleId(int roleId)
        {
            var criteria = new GetFunctionsByRoleIdCriteria { RoleId = roleId };
            IEnumerable<FunctionModel> functions = new List<FunctionModel>();

            if (criteria.IsValid())
            {
                functions = baseService.GetData<FunctionModel>(criteria);
                functions = functions.OrderBy(i => i.FunctionKey);
            }

            return functions;
        }

        public CommandResult AssignFunctionsToRole(AssignFunctionToRoleCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = commandService.Update(criteria);
            }

            return commandResult;
        }
    }
}