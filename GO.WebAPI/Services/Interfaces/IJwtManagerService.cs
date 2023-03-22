namespace GO.WebAPI.Services
{
    using System.Threading.Tasks;
    using GO.Models.Auth;

    public interface IJwtManagerService
    {
        Task<JwtToken> GenerateTokenAsync(int userId, short siteId);
    }
}