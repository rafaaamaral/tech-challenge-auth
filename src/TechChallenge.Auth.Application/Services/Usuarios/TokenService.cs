using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TechChallenge.Auth.Application.Services.Usuarios.Model;

namespace TechChallenge.Auth.Application.Services.Usuarios
{
    public class TokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GerarToken(UsuarioModel usuario)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var expiracaoConfig = jwtSettings["ExpirationMinutes"]
                                 ?? Environment.GetEnvironmentVariable("JWT_EXPIRATION_MINUTES");

            var expirationMinutes = int.TryParse(expiracaoConfig, out var parsedExpirationMinutes)
                ? parsedExpirationMinutes
                : 120;

            var secret = jwtSettings["Secret"]
                        ?? Environment.GetEnvironmentVariable("JWT_SECRET")
                        ?? "ChaveSuperSecretaTechChallengeFase3_256bits+";

            var issuer = jwtSettings["Issuer"]
                        ?? Environment.GetEnvironmentVariable("JWT_ISSUER")
                        ?? "TechChallenge.Auth";

            var audience = jwtSettings["Audience"]
                          ?? Environment.GetEnvironmentVariable("JWT_AUDIENCE")
                          ?? "TechChallenge.Api";

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.UniqueCode.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nome),
                new Claim("Documento", usuario.Documento),
                new Claim(ClaimTypes.Email, usuario.Login),
                new Claim(ClaimTypes.Role, usuario.Perfil.ToString())
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(secret)
            );

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
