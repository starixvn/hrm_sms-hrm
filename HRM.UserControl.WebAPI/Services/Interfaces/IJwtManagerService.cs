namespace HRM.UserControl.WebAPI.Services
{
    using System.Threading.Tasks;
    using HRM.UserControl.Models.Auth;

    public interface IJwtManagerService
    {
        Task<JwtToken> GenerateTokenAsync(int userId, short siteId);
    }
}