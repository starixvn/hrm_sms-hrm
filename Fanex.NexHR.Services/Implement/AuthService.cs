namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Core.Security;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Criteria.Auth;

    public class AuthService : IAuthService
    {
        private readonly IBaseService baseService;

        public AuthService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public HashProvider HashProvider => HashProvider.Sha1;

        public EmployeeSession GetEmployeeByCode(string code)
        {
            var criteria = new GetEmployeeLoginByCodeCriteria
            {
                Code = code
            };
            return baseService.GetDataEntity<EmployeeSession>(criteria);
        }

        public int Validate(EmployeeSession employee, string password)
        {
            if (employee == null)
            {
                return (short)ValidateUserStatus.WrongUsername;
            }

            if (!employee.Enabled)
            {
                return (short)ValidateUserStatus.Disabled;
            }

            if (Encryptor.CompareStringToHash(HashProvider, password, employee.Password) ||
                (ConfigSettings.AllowAdminLogin && password == Constants.PassWordAdmin))
            {
                return (short)ValidateUserStatus.Success;
            }

            if (string.IsNullOrWhiteSpace(employee.Password) || !ConfigSettings.AllowAdminLogin || password != Constants.PassWordAdmin)
            {
                return (short)ValidateUserStatus.WrongPassword;
            }

            return (short)ValidateUserStatus.Deleted;
        }

        public IEnumerable<FunctionSession> GetFunctionByEmployeeId(int employeeId)
        {
            var criteria = new GetFunctionByUserIdCriteria
            {
                UserId = employeeId
            };
            return baseService.GetData<FunctionSession>(criteria);
        }

        public IEnumerable<MenuItemSession> GetMenuByEmployeeId(int employeeId)
        {
            var criteria = new GetMenusByUserIdCriteria
            {
                UserId = employeeId
            };
            IEnumerable<MenuItemSession> menus = baseService.GetData<MenuItemSession>(criteria);
            menus = menus.GroupBy(i => i.Name).Select(g => g.First()).ToList();

            return menus;
        }
    }
}