using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TechChallenge.Auth.Application.Interfaces.Repositories;
using TechChallenge.Auth.Domain.Aggregates.Usuarios;
using TechChallenge.Auth.Infrastructure.Context;
using TechChallenge.Auth.Infrastructure.Repositories.Base;

namespace TechChallenge.Auth.Infrastructure.Repositories
{
    public class UsuarioRepository : BaseRepository<Usuario>, IUsuarioRepository
    {
        public UsuarioRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Usuario?> ObterPorDocumentoAsync(string documento)
        {
            return await _context.Usuarios.FirstOrDefaultAsync(u => u.Documento == documento);
        }
    }
}
