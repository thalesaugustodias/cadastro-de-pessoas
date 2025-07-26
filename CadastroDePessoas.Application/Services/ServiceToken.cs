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
        private string ObterChaveJwt()
        {
            // Prioriza variável de ambiente, depois appsettings
            var chave = Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ?? configuration["Jwt:Chave"];

            if (string.IsNullOrEmpty(chave))
            {
                throw new InvalidOperationException("Chave JWT não configurada. Configure a variável de ambiente JWT_SECRET_KEY ou a configuração Jwt:Chave.");
            }

            return chave;
        }

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

            var chaveSecreta = ObterChaveJwt();
            var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chaveSecreta));
            var credenciais = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);
            var expiracao = DateTime.UtcNow.AddHours(double.Parse(configuration["Jwt:ExpiracionHoras"] ?? "1"));

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
            var chaveSecreta = ObterChaveJwt();
            var chave = Encoding.UTF8.GetBytes(chaveSecreta);

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
