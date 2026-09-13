using System;
using System.Collections.Generic;
using System.Text;
using TechChallenge.Auth.Application.Exceptions;
using TechChallenge.Auth.Application.Interfaces.Repositories;
using TechChallenge.Auth.Application.Interfaces.Services;
using TechChallenge.Auth.Application.Services.Usuarios.Model;

namespace TechChallenge.Auth.Application.Services.Usuarios
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioService(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<UsuarioModel> ObterPorDocumentoAsync(string documento)
        {
            var usuario = await _usuarioRepository.ObterPorDocumentoAsync(documento);

            if (usuario == null)
                throw new NotFoundException("Usuário", documento);

            return new UsuarioModel
            {
                UniqueCode = usuario.UniqueCode,
                Nome = usuario.Nome,
                Login = usuario.Login,
                Documento = usuario.Documento,
                Senha = usuario.Senha,
                Perfil = usuario.Perfil,
                Ativo = usuario.Ativo
            };
        }
    }
}
