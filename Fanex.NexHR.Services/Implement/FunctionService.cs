namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Core.XMLExtensions;
    using Fanex.NexHR.Criteria.Function;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Function;

    public class FunctionService : IFunctionService
    {
        private readonly IBaseService baseService;

        public FunctionService(IBaseService baseService)
        {
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
            return baseService.GetData<FunctionModel>(criteria);
        }

        public IEnumerable<FunctionModel> GetFunctionsByRoleId(int roleId)
        {
            var criteria = new GetFunctionsByRoleIdCriteria { RoleId = roleId };
            IEnumerable<FunctionModel> functions = baseService.GetData<FunctionModel>(criteria);
            functions = functions.OrderBy(i => i.FunctionKey);

            return functions;
        }

        public CommandResult AssignFunctionsToRole(int actorId, int roleId, IEnumerable<FunctionPermission> permissions)
        {
            var criteria = new AssignFunctionToRoleCriteria
            {
                RoleId = roleId,
                ActorId = actorId,
                PermissionXml = permissions.ToXML().ToString()
            };

            return baseService.ExecuteNonQuery(criteria);
        }
    }
}