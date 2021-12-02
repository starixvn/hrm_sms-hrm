namespace HRM.UserControl.API.Core.Services
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;
    using HRM.UserControl.API.Core.Models;
    using HRM.UserControl.Models.Auth;
    using Microsoft.IdentityModel.Tokens;

    public interface IJwtManagerService
    {
        Task<JwtToken> GenerateTokenAsync(int userId, short siteId);
    }

    public class JwtManagerService : IJwtManagerService
    {
        private readonly IUserService userService;
        private readonly ISiteService siteService;
        private readonly AuthenticationSettings settings;

        public JwtManagerService(
            IUserService userService,
            ISiteService siteService,
            AuthenticationSettings settings)
        {
            this.userService = userService;
            this.siteService = siteService;
            this.settings = settings;
        }

        public async Task<JwtToken> GenerateTokenAsync(int userId, short siteId)
        {
            var jwtToken = new JwtToken();
            if (userId > 0 && siteId > 0)
            {
                var user = await userService.GetByIdAsync(userId);
                var site = await siteService.GetByIdAsync(siteId);

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(settings.SecretKey);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim("UserId", user.UserId.ToString()),
                        new Claim("SiteId", site.SiteId.ToString()),
                        new Claim(nameof(site.AccessToken), site.AccessToken)
                    }),
                    Expires = DateTime.UtcNow.AddMinutes(settings.Expiration),
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                jwtToken.Token = tokenHandler.WriteToken(token);
                jwtToken.ValidFrom = token.ValidFrom;
                jwtToken.ValidTo = token.ValidTo;
            }

            return jwtToken;
        }
    }
}