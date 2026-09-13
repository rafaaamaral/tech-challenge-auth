using System;
using System.Collections.Generic;
using System.Text;
using TechChallenge.Auth.Application.Interfaces.Repositories.Base;
using TechChallenge.Auth.Domain.Aggregates.Usuarios;

namespace TechChallenge.Auth.Application.Interfaces.Repositories
{
    public interface IUsuarioRepository : IBaseRepository<Usuario>
    {
        Task<Usuario?> ObterPorDocumentoAsync(string documento);
    }
}
