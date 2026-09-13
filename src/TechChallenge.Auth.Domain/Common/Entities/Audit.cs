using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace TechChallenge.Auth.Domain.Common.Entities
{
    [ExcludeFromCodeCoverage]
    public class Audit : BaseDomain
    {
        public bool Ativo { get; set; }
        public Guid CriadoPor { get; set; }
        public DateTime DataCriacao { get; set; }
        public Guid? AlteradoPor { get; set; }
        public DateTime? DataAlteracao { get; set; }
    }
}
