namespace HRM.UserControl.Client.Core.Services.Interfaces
{
    using System.Threading.Tasks;
    using HRM.UserControl.Client.Core.Models;

    public interface IUserService
    {
        User GetById(int userId);

        Task<User> GetByIdAsync(int userId);

        User GetByUsername(string username);

        Task<User> GetByUsernameAsync(string username);
    }
}