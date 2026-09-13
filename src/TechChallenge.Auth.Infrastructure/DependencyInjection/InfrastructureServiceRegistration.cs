using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using TechChallenge.Auth.Application.Interfaces.Repositories;
using TechChallenge.Auth.Application.Interfaces.Services;
using TechChallenge.Auth.Application.Services.Usuarios;
using TechChallenge.Auth.Infrastructure.Context;
using TechChallenge.Auth.Infrastructure.Repositories;

namespace TechChallenge.Auth.Infrastructure.DependencyInjection
{
    [ExcludeFromCodeCoverage]
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("A connection string 'DefaultConnection' não foi informada.");
            }

            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString));

            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<IUsuarioService, UsuarioService>();

            return services;
        }
    }
}
