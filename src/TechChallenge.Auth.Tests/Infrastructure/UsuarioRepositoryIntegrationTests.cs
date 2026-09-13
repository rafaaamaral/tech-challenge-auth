using Microsoft.EntityFrameworkCore;
using TechChallenge.Auth.Domain.Aggregates.Usuarios;
using TechChallenge.Auth.Domain.Common.Enums;
using TechChallenge.Auth.Infrastructure.Context;
using TechChallenge.Auth.Infrastructure.Repositories;

namespace TechChallenge.Auth.Tests.Infrastructure;

public class UsuarioRepositoryIntegrationTests
{
    [Fact]
    public async Task ObterPorDocumentoAsync_DeveRetornarUsuario_QuandoDocumentoExistir()
    {
        await using var contexto = CriarContexto();
        contexto.Usuarios.Add(new Usuario
        {
            Nome = "Cliente",
            Login = "cliente@teste.com",
            Documento = "52998224725",
            Senha = "hash",
            Perfil = PerfilUsuario.Cliente,
            Ativo = true,
            CriadoPor = Guid.NewGuid(),
            DataCriacao = DateTime.UtcNow
        });
        await contexto.SaveChangesAsync();

        var repositorio = new UsuarioRepository(contexto);

        var usuario = await repositorio.ObterPorDocumentoAsync("52998224725");

        Assert.NotNull(usuario);
        Assert.Equal("52998224725", usuario.Documento);
    }

    [Fact]
    public async Task ObterPorDocumentoAsync_DeveRetornarNull_QuandoDocumentoNaoExistir()
    {
        await using var contexto = CriarContexto();
        var repositorio = new UsuarioRepository(contexto);

        var usuario = await repositorio.ObterPorDocumentoAsync("00000000000");

        Assert.Null(usuario);
    }

    private static AppDbContext CriarContexto()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }
}
