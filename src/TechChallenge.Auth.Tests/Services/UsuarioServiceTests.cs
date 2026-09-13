using System.Linq.Expressions;
using TechChallenge.Auth.Application.Exceptions;
using TechChallenge.Auth.Application.Interfaces.Repositories;
using TechChallenge.Auth.Application.Services.Usuarios;
using TechChallenge.Auth.Domain.Aggregates.Usuarios;
using TechChallenge.Auth.Domain.Common.Enums;

namespace TechChallenge.Auth.Tests.Services;

public class UsuarioServiceTests
{
    [Fact]
    public async Task ObterPorDocumentoAsync_DeveMapearUsuario_QuandoEncontrado()
    {
        var usuario = new Usuario
        {
            UniqueCode = Guid.NewGuid(),
            Nome = "Rafael",
            Login = "rafael@teste.com",
            Documento = "52998224725",
            Senha = "hash",
            Perfil = PerfilUsuario.Cliente,
            Ativo = true
        };

        var repositorio = new UsuarioRepositoryFake(usuario);
        var service = new UsuarioService(repositorio);

        var resultado = await service.ObterPorDocumentoAsync("52998224725");

        Assert.Equal(usuario.UniqueCode, resultado.UniqueCode);
        Assert.Equal(usuario.Documento, resultado.Documento);
        Assert.Equal(usuario.Ativo, resultado.Ativo);
        Assert.Equal(usuario.Perfil, resultado.Perfil);
    }

    [Fact]
    public async Task ObterPorDocumentoAsync_DeveLancarNotFoundException_QuandoNaoEncontrado()
    {
        var repositorio = new UsuarioRepositoryFake(null);
        var service = new UsuarioService(repositorio);

        await Assert.ThrowsAsync<NotFoundException>(() => service.ObterPorDocumentoAsync("00000000000"));
    }

    private sealed class UsuarioRepositoryFake : IUsuarioRepository
    {
        private readonly Usuario? _usuario;

        public UsuarioRepositoryFake(Usuario? usuario)
        {
            _usuario = usuario;
        }

        public Task<Usuario?> ObterPorDocumentoAsync(string documento)
            => Task.FromResult(_usuario?.Documento == documento ? _usuario : null);

        public Task<Usuario?> GetByIdAsync(int id) => Task.FromResult<Usuario?>(null);
        public Task<Usuario?> GetByUniqueCodeAsync(Guid uniqueCode) => Task.FromResult<Usuario?>(null);
        public Task<IEnumerable<Usuario>> GetAllAsync() => Task.FromResult(Enumerable.Empty<Usuario>());
        public Task<IEnumerable<Usuario>> FindAsync(Expression<Func<Usuario, bool>> predicate) => Task.FromResult(Enumerable.Empty<Usuario>());
        public Task<Usuario> AddAsync(Usuario entity) => Task.FromResult(entity);
        public Task UpdateAsync(Usuario entity) => Task.CompletedTask;
        public Task RemoveAsync(Usuario entity) => Task.CompletedTask;
        public Task<bool> ExistsAsync(Expression<Func<Usuario, bool>> predicate) => Task.FromResult(false);
        public Task<int> CountAsync() => Task.FromResult(0);
    }
}
