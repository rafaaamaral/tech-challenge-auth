using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace TechChallenge.Auth.Utils
{
    [ExcludeFromCodeCoverage]
    public class JwtTokenGenerator
    {
        public static string GenerateToken(string cpf)
        {
            var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET") ?? "ChaveSuperSecretaTechChallengeFase3_256bits+";
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, cpf),
            new Claim("role", "Atendimento") 
        };

            var token = new JwtSecurityToken(
                issuer: Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "TechChallenge.Auth",
                audience: Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "TechChallenge.Api",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
