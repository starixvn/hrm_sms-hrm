namespace GO.Website.Controllers.API
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using GO.Website.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.IdentityModel.Tokens;

    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly JwtAuthSettings authSettings;

        public TokenController(JwtAuthSettings authSettings)
        {
            this.authSettings = authSettings;
        }

        [HttpPost]
        public IActionResult GetToken(JwtAuthSettings model)
        {
            if (IsValid(model.Username, model.Password))
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(authSettings.SecretKey);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[] {
                        new Claim(ClaimTypes.Name, authSettings.Username)
                    }),
                    Expires = DateTime.UtcNow.AddMinutes(authSettings.Expiration),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var sercurityToken = tokenHandler.CreateToken(tokenDescriptor);
                var token = tokenHandler.WriteToken(sercurityToken);

                return Ok(token);
            }

            return Unauthorized();
        }

        public bool IsValid(string username, string password)
        {
            return username == authSettings.Username && password == authSettings.Password;
        }
    }
}