using System.ComponentModel;

namespace TechChallenge.Auth.Domain.Common.Enums
{
    public enum PerfilUsuario
    {
        [Description("Administrador")]
        Administrador = 1,
        [Description("Atendimento")]
        Atendimento = 2,
        [Description("Mecânico")]
        Mecanico = 3,
        [Description("Cliente")]
        Cliente = 4
    }
}
