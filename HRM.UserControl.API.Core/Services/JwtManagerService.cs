using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using HRM.UserControl.API.Core.Services.Interfaces;
using IUC.API.Core.Models;
using Microsoft.IdentityModel.Tokens;

namespace HRM.UserControl.API.Core.Services
{
    public interface IJwtManagerService
    {
        Task<string> GenerateToken(int userId, int siteId);
    }

    public class JwtManagerService : IJwtManagerService
    {
        private readonly IUserService userService;
        private readonly ISiteService siteService;
        private readonly AuthenticationSettings settings;

        public JwtManagerService(IUserService userService,
            ISiteService siteService,
            AuthenticationSettings settings)
        {
            this.userService = userService;
            this.siteService = siteService;
            this.settings = settings;
        }

        public async Task<string> GenerateToken(int userId, int siteId)
        {
            string tokenValue = string.Empty;
            if (userId > 0 && siteId > 0)
            {
                var user = await userService.GetUserByIdAsync(userId);
                var site = await siteService.GetSiteById(siteId);

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(settings.SecretKey);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, user.NickName),
                    new Claim("SiteId", site.SiteId.ToString()),
                    new Claim(nameof(site.AccessToken), site.AccessToken),
                    }),
                    Expires = DateTime.UtcNow.AddMinutes(settings.Expiration),
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                tokenValue = tokenHandler.WriteToken(token);
            }

            return tokenValue;
        }
    }
}