using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace TechChallenge.Auth.Domain.Common.Entities
{
    [ExcludeFromCodeCoverage]
    public class BaseDomain
    {
        public int Id { get; set; }
        public Guid UniqueCode { get; set; } = Guid.NewGuid();
    }
}
