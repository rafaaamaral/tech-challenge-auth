using System;
using System.Collections.Generic;
using System.Text;
using TechChallenge.Auth.Application.Services.Usuarios.Model;

namespace TechChallenge.Auth.Application.Interfaces.Services
{
    public interface IUsuarioService
    {
        Task<UsuarioModel> ObterPorDocumentoAsync(string documento);
    }
}
