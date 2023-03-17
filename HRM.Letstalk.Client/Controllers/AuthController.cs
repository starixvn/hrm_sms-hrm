namespace HRM.Letstalk.Client.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using System;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.IdentityModel.Tokens;
    using HRM.Letstalk.Client.Models;

    public class AuthController : ControllerBase
    {
        private readonly AuthenticationSettings appSettings;
        private readonly string invalidUsernameOrPasswordText = "Username or password is incorrect";
        private JwtToken jwt;

        public AuthController(AuthenticationSettings appSettings)
        {
            this.appSettings = appSettings;
            jwt = null;
        }


        [AllowAnonymous]
        [HttpGet]
        [Route("api/authenticate")]
        public ObjectResult Authenticate(string username, string password)
        {
            if (IsValidCredential(username, password))
            {
                var jwtToken = GetAccessToken(username);
                return Ok(jwtToken);
            }
            else
            {
                return BadRequest(invalidUsernameOrPasswordText);
            }
        }

        private JwtToken GetAccessToken(string username)
        {
            if (jwt == null || string.IsNullOrEmpty(jwt.Token) || jwt.IsExpired)
            {
                jwt = GenerateAccessToken(username);
            }
            return jwt;
        }

        private JwtToken GenerateAccessToken(string username)
        {
            var jwtToken = new JwtToken();
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(appSettings.SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                        new Claim(ClaimTypes.Name, username)
                    }),
                Expires = DateTime.UtcNow.AddMinutes(appSettings.Expiration),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            jwtToken.Token = tokenHandler.WriteToken(token);
            jwtToken.ValidFrom = token.ValidFrom;
            jwtToken.ValidTo = token.ValidTo;

            return jwtToken;
        } 

        private bool IsValidCredential(string username, string password)
        {
            return username.Equals(appSettings.AppCredentialId, StringComparison.InvariantCultureIgnoreCase)
                && password.Equals(appSettings.AppCredentialPassword, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
