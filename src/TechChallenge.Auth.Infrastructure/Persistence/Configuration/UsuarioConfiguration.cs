using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TechChallenge.Auth.Domain.Aggregates.Usuarios;
using TechChallenge.Auth.Infrastructure.Persistence.Configuration.Base;

namespace TechChallenge.Auth.Infrastructure.Persistence.Configuration
{
    public class UsuarioConfiguration : AuditConfiguration<Usuario>
    {
        public override void Configure(EntityTypeBuilder<Usuario> builder)
        {
            base.Configure(builder);

            builder.ToTable("Usuario");

            builder.Property(c => c.Nome).IsRequired().HasMaxLength(100);
            builder.Property(c => c.Login).IsRequired().HasMaxLength(50);
            builder.Property(c => c.Documento).IsRequired().HasMaxLength(50);
            builder.Property(c => c.Senha).IsRequired().HasMaxLength(200);
            builder.Property(c => c.Perfil).IsRequired().HasConversion<int>();
        }
    }
}
