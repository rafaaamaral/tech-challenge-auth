using System;
using System.Collections.Generic;
using System.Text;
using TechChallenge.Auth.Domain.Common.Entities;
using TechChallenge.Auth.Domain.Common.Enums;

namespace TechChallenge.Auth.Domain.Aggregates.Usuarios
{
    public class Usuario : Audit
    {
        public string Nome { get; set; } = string.Empty;
        public string Login { get; set; } = string.Empty;
        public string Documento { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public PerfilUsuario Perfil { get; set; }

        public Usuario() { }
    }
}
