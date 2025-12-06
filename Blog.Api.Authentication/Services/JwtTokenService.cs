using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Blog.Api.Authentication.Services
{
    public class JwtTokenService
    {
        private readonly IConfiguration _config;
        private readonly string _key;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _durationInMinutes;

        public JwtTokenService(IConfiguration config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));

            // Ler a seção JwtSettings (consistente com appsettings.json)
            var jwtSection = _config.GetSection("JwtSettings");
            _key = jwtSection["Key"] ?? throw new Exception("JWT Key não configurada.");
            _issuer = jwtSection["Issuer"] ?? throw new Exception("JWT Issuer não configurado.");
            _audience = jwtSection["Audience"] ?? throw new Exception("JWT Audience não configurado.");

            var durationStr = jwtSection["DurationInMinutes"];
            if (!int.TryParse(durationStr, out _durationInMinutes))
            {
                _durationInMinutes = 60; // fallback razoável
            }
        }

        public string GenerateToken(string userId, string username, string email)
        {
            var keyBytes = Encoding.UTF8.GetBytes(_key);
            var key = new SymmetricSecurityKey(keyBytes);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.UniqueName, username ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Email, email ?? string.Empty),
                // você pode adicionar roles ou outros claims aqui se quiser
            };

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_durationInMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
