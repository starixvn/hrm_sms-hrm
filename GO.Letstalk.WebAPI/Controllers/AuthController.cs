namespace GO.Letstalk.WebAPI.Controllers
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using GO.Letstalk.WebAPI.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.IdentityModel.Tokens;

    public class AuthController : ControllerBase
    {
        private readonly JwtAuthSettings jwtAuthSettings;
        private JwtToken jwt;

        public AuthController(JwtAuthSettings jwtAuthSettings)
        {
            this.jwtAuthSettings = jwtAuthSettings;
            jwt = null;
        }

        [HttpGet]
        [Route("api/authenticate")]
        public ObjectResult Authenticate(string username, string password)
        {
            if (IsValidCredential(username, password))
            {
                var jwtToken = GetAccessToken(username);
                return Ok(jwtToken);
            }

            return BadRequest("Username or password is incorrect");
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
            var key = Encoding.ASCII.GetBytes(jwtAuthSettings.SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, username)
                }),
                Expires = DateTime.UtcNow.AddMinutes(jwtAuthSettings.Expiration),
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
            return username.Equals(jwtAuthSettings.Username, StringComparison.InvariantCultureIgnoreCase)
                && password.Equals(jwtAuthSettings.Password, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}