using CadastroDePessoas.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CadastroDePessoas.Application.Services
{
    public class ServiceToken(IConfiguration configuration) : IServiceToken
    {
        public string GerarToken(Guid usuarioId, string email, string nome, IEnumerable<string> roles = null)
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, usuarioId.ToString()),
                new(JwtRegisteredClaimNames.Email, email),
                new(JwtRegisteredClaimNames.Name, nome),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            if (roles != null)
            {
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }

            var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Chave"]));
            var credenciais = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);
            var expiracao = DateTime.UtcNow.AddHours(double.Parse(configuration["Jwt:ExpiracionHoras"]));

            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Emissor"],
                audience: configuration["Jwt:Audiencia"],
                claims: claims,
                expires: expiracao,
                signingCredentials: credenciais
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ClaimsPrincipal ValidarToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var chave = Encoding.ASCII.GetBytes(configuration["Jwt:Chave"]);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(chave),
                ValidateIssuer = true,
                ValidIssuer = configuration["Jwt:Emissor"],
                ValidateAudience = true,
                ValidAudience = configuration["Jwt:Audiencia"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            return tokenHandler.ValidateToken(token, validationParameters, out _);
        }
    }
}
