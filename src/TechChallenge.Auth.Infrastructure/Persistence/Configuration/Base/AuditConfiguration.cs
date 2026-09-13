using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TechChallenge.Auth.Domain.Common.Entities;

namespace TechChallenge.Auth.Infrastructure.Persistence.Configuration.Base
{
    public abstract class AuditConfiguration<T> : IEntityTypeConfiguration<T> where T : Audit
    {
        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(c => c.Id).IsRequired().UseIdentityAlwaysColumn();
            builder.Property(c => c.UniqueCode).IsRequired();
            builder.Property(c => c.Ativo).IsRequired();
            builder.Property(c => c.CriadoPor).IsRequired();
            builder.Property(c => c.DataCriacao).IsRequired();
            builder.Property(c => c.AlteradoPor);
            builder.Property(c => c.DataAlteracao);
        }
    }
}
