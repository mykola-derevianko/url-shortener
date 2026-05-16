using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using URLShortener.Application.Interfaces;
using URLShortener.Domain.Entities;

namespace URLShortener.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _key;

        public TokenService(IConfiguration config)
        {
            _config = config;

            var tokenKey = _config["Jwt:Key"] ?? throw new ArgumentNullException("Jwt:Key", "JWT Key is missing in the configuration.");
            var issuer = _config["Jwt:Issuer"] ?? throw new ArgumentNullException("Jwt:Issuer", "JWT Issuer is missing in the configuration.");
            var audience = _config["Jwt:Audience"] ?? throw new ArgumentNullException("Jwt:Audience", "JWT Audience is missing in the configuration.");

            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));
        }

        public string GenerateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            if (!double.TryParse(_config["Jwt:ExpireDays"], out double expireDays))
            {
                expireDays = 7;
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(expireDays),
                SigningCredentials = creds,
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}