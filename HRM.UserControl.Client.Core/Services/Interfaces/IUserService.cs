namespace HRM.UserControl.Client.Core.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRM.UserControl.Client.Core.Models;
    using HRM.UserControl.Client.Core.Models.Role;

    public interface IUserService
    {
        Task<User> GetByIdAsync(int userId);

        Task<User> GetByEmailAsync(string email);

        Task<IList<CompanyRoleFunction>> GetPermissionAsync();

        Task<CompanyRoleFunction> GetPermissionOnCompanyAsync(int companyId);

        Task<IList<Company>> GetCompanyAsync();
    }
}