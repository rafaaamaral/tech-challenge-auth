using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using TechChallenge.Auth.Application.Services.Usuarios;
using TechChallenge.Auth.Application.Services.Usuarios.Model;
using TechChallenge.Auth.Domain.Common.Enums;

namespace TechChallenge.Auth.Tests.Services;

public class TokenServiceTests
{
    [Fact]
    public void GerarToken_DeveGerarTokenComClaimsEsperadas_QuandoConfiguracaoJwtExiste()
    {
        var configuracao = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Secret"] = "ChaveSuperSecretaTechChallengeFase3_256bits+",
                ["Jwt:Issuer"] = "TechChallenge.Auth",
                ["Jwt:Audience"] = "TechChallenge.Api",
                ["Jwt:ExpirationMinutes"] = "120"
            })
            .Build();

        var service = new TokenService(configuracao);
        var usuario = CriarUsuario();

        var token = service.GerarToken(usuario);

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        Assert.Equal("TechChallenge.Auth", jwt.Issuer);
        Assert.Contains("TechChallenge.Api", jwt.Audiences);
        Assert.Contains(jwt.Claims, c => c.Type == "Documento" && c.Value == usuario.Documento);
        Assert.Contains(jwt.Claims, c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name" && c.Value == usuario.Nome);
    }

    [Fact]
    public void GerarToken_DeveUsarVariaveisAmbiente_QuandoConfiguracaoJwtNaoExiste()
    {
        Environment.SetEnvironmentVariable("JWT_SECRET", "OutraChaveSuperSecretaTechChallengeFase3_256bits+");
        Environment.SetEnvironmentVariable("JWT_ISSUER", "Issuer.Env");
        Environment.SetEnvironmentVariable("JWT_AUDIENCE", "Audience.Env");
        Environment.SetEnvironmentVariable("JWT_EXPIRATION_MINUTES", "60");

        try
        {
            var configuracao = new ConfigurationBuilder().Build();
            var service = new TokenService(configuracao);

            var token = service.GerarToken(CriarUsuario());

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            Assert.Equal("Issuer.Env", jwt.Issuer);
            Assert.Contains("Audience.Env", jwt.Audiences);
        }
        finally
        {
            Environment.SetEnvironmentVariable("JWT_SECRET", null);
            Environment.SetEnvironmentVariable("JWT_ISSUER", null);
            Environment.SetEnvironmentVariable("JWT_AUDIENCE", null);
            Environment.SetEnvironmentVariable("JWT_EXPIRATION_MINUTES", null);
        }
    }

    private static UsuarioModel CriarUsuario()
    {
        return new UsuarioModel
        {
            UniqueCode = Guid.NewGuid(),
            Nome = "Usuário Teste",
            Login = "teste@local",
            Documento = "52998224725",
            Senha = "123",
            Perfil = PerfilUsuario.Atendimento,
            Ativo = true
        };
    }
}
