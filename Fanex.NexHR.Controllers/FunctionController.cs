namespace Fanex.NexHR.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Filters;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Criteria.Function;
    using Fanex.NexHR.ViewModels.Function;
    using HRM.UserControl.Client.Core.Enumerables;
    using HRM.UserControl.Client.Core.Models;
    using HRM.UserControl.Client.Core.Models.Role;
    using HRM.UserControl.Client.Core.Services;
    using HRM.UserControl.Client.Core.Session;

    public class FunctionController : BaseController
    {
        private readonly IUserControlSession userControlSession;
        private readonly IBaseService baseService;
        private readonly IRoleService roleService;
        private readonly IFunctionService ucfunctionService;

        public FunctionController(
            ISessionService sessionService,
            IUserControlSession userControlSession,
            IBaseService baseService,
            IRoleService roleService,
            IFunctionService ucfunctionService) : base(sessionService)
        {
            this.userControlSession = userControlSession;
            this.baseService = baseService;
            this.roleService = roleService;
            this.ucfunctionService = ucfunctionService;
        }

        [HRMPermission(Policy.Read, FunctionKey.FunctionManagement)]
        public ActionResult Index(short? roleId = null)
        {
            IList<Role> systemRoles = roleService.Get(true);
            IList<Function> functions = ucfunctionService.Get(true);
            var functionTrees = new TreeFunctionViewModel
            {
                RoleId = roleId ?? systemRoles.First().RoleId,
                IsHost = userControlSession.HasPermissionOnFunction(FunctionKey.FunctionManagement, Policy.Host),
                Functions = GetFunctionTrees(functions),
                FunctionPermissions = ucfunctionService.GetPermissionByRoleId(roleId ?? systemRoles.First().RoleId)
            };

            ViewBag.SystemRoles = systemRoles;

            return View(functionTrees);
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.FunctionManagement)]
        public CustomJsonResult AssignPermission(short roleId, IList<FunctionPermission> permissions)
        {
            CommandResult commandResult = roleService.AssignFunction(roleId, permissions);
            if (commandResult.IsSuccessful)
            {
                var permissionXml = new StringBuilder("<Root>");
                if (permissions != null && permissions.Any())
                {
                    const string AssignPermissionTpl = "<Row FunctionId='{0}' IsRead='{1}' IsHost='{2}' />";
                    foreach (var permission in permissions)
                    {
                        permissionXml.AppendFormat(AssignPermissionTpl, permission.FunctionId, permission.IsRead, permission.IsHost);
                    }
                }

                var criteria = new AssignFunctionToRoleCriteria
                {
                    RoleId = roleId,
                    ActorId = sessionService.Employee.Id,
                    PermissionXml = permissionXml.Append("</Root>").ToString()
                };

                var result = baseService.ExecuteNonQuery(criteria);
                return new CustomJsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        private IList<Function> GetFunctionTrees(IList<Function> functions)
        {
            IList<Function> functionTrees = functions.Where(func => !func.ParentId.HasValue).ToList();
            foreach (Function function in functionTrees)
            {
                function.Childs = new List<Function>();
                function.Level = 0;
                GetChildFunctions(function, functions);
            }

            return functionTrees;
        }

        private void GetChildFunctions(Function function, IList<Function> functions)
        {
            IList<Function> functionItems = functions.Where(x => x.ParentId == function.FunctionId).ToList();
            if (functionItems.Any())
            {
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