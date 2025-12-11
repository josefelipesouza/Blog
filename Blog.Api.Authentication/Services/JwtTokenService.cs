// JwtTokenService.cs

using Microsoft.AspNetCore.Identity;
using Blog.Api.Authentication.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims; // Contém ClaimTypes.Role
using System.Text;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blog.Api.Authentication.Services
{
    // Certifique-se de que a entidade ApplicationUser está definida em Blog.Api.Authentication.Entities
    // usando 'using Blog.Api.Authentication.Entities;'

    public class JwtTokenService
    {
        private readonly IConfiguration _config;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly string _key;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _durationInMinutes;

        // Construtor com UserManager injetado
        public JwtTokenService(IConfiguration config, UserManager<ApplicationUser> userManager)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));

            var jwtSection = _config.GetSection("JwtSettings");

            _key = jwtSection["Key"] ?? throw new Exception("JWT Key não configurada.");
            _issuer = jwtSection["Issuer"] ?? throw new Exception("JWT Issuer não configurado.");
            _audience = jwtSection["Audience"] ?? throw new Exception("JWT Audience não configurado.");

            if (!int.TryParse(jwtSection["DurationInMinutes"], out _durationInMinutes))
                _durationInMinutes = 60;
        }

        // Gera o token JWT incluindo roles
        public async Task<string> GenerateToken(ApplicationUser user)
        {
            // O mapeamento de claims já deve ter sido limpo no Program.cs
            // Manter a linha aqui como redundância, mas não deve ser estritamente necessário:
            JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();
            
            var keyBytes = Encoding.UTF8.GetBytes(_key);
            var signingKey = new SymmetricSecurityKey(keyBytes);
            var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var roles = await _userManager.GetRolesAsync(user);
            
            // Claims básicas (JwtRegisteredClaimNames já resolvem o mapeamento)
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Adiciona roles às claims
            if (roles != null && roles.Any())
            {
                foreach (var role in roles)
                {
                    // 1. Adiciona a role usando o ClaimTypes.Role (formato longo do Identity)
                    // Este é o mais seguro para garantir que o Identity a reconheça
                    claims.Add(new Claim(ClaimTypes.Role, role));
                    
                    // 2. Adiciona a role usando "rol" (padrão IANA, configurado no Program.cs)
                    claims.Add(new Claim("rol", role));
                    
                    // 3. Adiciona a role usando "role" (formato curto, para debug/compatibilidade)
                    claims.Add(new Claim("role", role));
                }
            }
            
            var now = DateTime.UtcNow;
            var expires = now.AddMinutes(_durationInMinutes);

            // Cria o payload e o token
            var payload = new JwtPayload(_issuer, _audience, claims, now, expires);
            var header = new JwtHeader(creds);
            var token = new JwtSecurityToken(header, payload);
            
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenString = tokenHandler.WriteToken(token);
            
            // **IMPORTANTE**: Removi a manipulação manual do payload.Remove/payload.Add, 
            // pois ela pode sobrescrever o comportamento desejado se você estiver 
            // usando um array de claims. Ao adicionar na lista 'claims', o payload
            // deve incluir a informação corretamente.
            
            return tokenString;
        }
    }
}