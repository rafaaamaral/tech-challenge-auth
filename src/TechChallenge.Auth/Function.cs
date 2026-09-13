using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;
using System.Text.Json;
using TechChallenge.Auth.Application.Exceptions;
using TechChallenge.Auth.Application.Interfaces.Services;
using TechChallenge.Auth.Application.Services.Usuarios;
using TechChallenge.Auth.Application.Services.Usuarios.Model;
using TechChallenge.Auth.Models;
using TechChallenge.Auth.Infrastructure.DependencyInjection;
using TechChallenge.Auth.Utils;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace TechChallenge.Auth;
[ExcludeFromCodeCoverage]
public class Function
{
    private readonly IUsuarioService? _usuarioService;
    private readonly TokenService _tokenService;

    public Function()
    {
        var configuration = BuildConfiguration();

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddScoped<TokenService>();

        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            var providerSemBanco = services.BuildServiceProvider();
            _tokenService = providerSemBanco.GetRequiredService<TokenService>();
            return;
        }

        services.AddInfrastructureServices(configuration);

        var provedorServicos = services.BuildServiceProvider();
        _usuarioService = provedorServicos.GetRequiredService<IUsuarioService>();
        _tokenService = provedorServicos.GetRequiredService<TokenService>();
    }

    private static IConfiguration BuildConfiguration()
    {
        var secretSettings = LoadDatabaseSettingsFromSecretsManager();

        var configurationBuilder = new ConfigurationBuilder()
            .AddEnvironmentVariables();

        if (secretSettings.Count > 0)
        {
            configurationBuilder.AddInMemoryCollection(secretSettings);
        }

        return configurationBuilder.Build();
    }

    private static Dictionary<string, string?> LoadDatabaseSettingsFromSecretsManager()
    {
        var secretName = Environment.GetEnvironmentVariable("DB_SECRET_NAME");

        if (string.IsNullOrWhiteSpace(secretName))
        {
            return new Dictionary<string, string?>();
        }

        using var secretsManager = new AmazonSecretsManagerClient();
        var secretValue = secretsManager.GetSecretValueAsync(new GetSecretValueRequest
        {
            SecretId = secretName
        }).GetAwaiter().GetResult();

        if (string.IsNullOrWhiteSpace(secretValue.SecretString))
        {
            throw new InvalidOperationException("O valor do secret está vazio.");
        }

        using var json = JsonDocument.Parse(secretValue.SecretString);
        var root = json.RootElement;

        if (TryGetString(root, "ConnectionString", out var connectionString) ||
            TryGetString(root, "connectionString", out connectionString))
        {
            return new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = connectionString
            };
        }

        var host = GetSecretValue(root, "host");
        var port = GetSecretValue(root, "port") ?? "5432";
        var database = GetSecretValue(root, "dbname") ?? GetSecretValue(root, "database");
        var username = GetSecretValue(root, "username");
        var password = GetSecretValue(root, "password");

        if (string.IsNullOrWhiteSpace(host) ||
            string.IsNullOrWhiteSpace(database) ||
            string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException("O secret de banco não contém campos suficientes do PostgreSQL.");
        }

        var builtConnectionString = $"Host={host};Port={port};Database={database};Username={username};Password={password};";

        return new Dictionary<string, string?>
        {
            ["ConnectionStrings:DefaultConnection"] = builtConnectionString
        };
    }

    private static bool TryGetString(JsonElement root, string propertyName, out string? value)
    {
        if (root.TryGetProperty(propertyName, out var property) && property.ValueKind == JsonValueKind.String)
        {
            value = property.GetString();
            return !string.IsNullOrWhiteSpace(value);
        }

        value = null;
        return false;
    }

    private static string? GetSecretValue(JsonElement root, string propertyName)
    {
        if (!root.TryGetProperty(propertyName, out var property))
        {
            return null;
        }

        return property.ValueKind switch
        {
            JsonValueKind.String => property.GetString(),
            JsonValueKind.Number => property.GetRawText(),
            _ => null
        };
    }

    public APIGatewayHttpApiV2ProxyResponse FunctionHandler(APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
    {
        Console.WriteLine($"[INICIO] RequestId: {context?.AwsRequestId}");

        // No HTTP API v2, o path vem em RawPath e o método vem dentro de RequestContext.Http
        var path = request.RawPath ?? request.RequestContext?.Http?.Path;
        var method = request.RequestContext?.Http?.Method;

        Console.WriteLine($"[REQUISICAO] Rota recebida: {path} | Método: {method}");
        Console.WriteLine($"[REQUISICAO] Body recebido: {request.Body}");

        if (method != "POST" || string.IsNullOrEmpty(path) || !path.EndsWith("/auth/cpf"))
        {
            Console.WriteLine("[AVISO] Rota ou método inválido para autenticação por CPF. Retornando 404.");
            return new APIGatewayHttpApiV2ProxyResponse { StatusCode = 404 };
        }

        if (string.IsNullOrWhiteSpace(request.Body))
        {
            Console.WriteLine("[AVISO] Body da requisição não informado. Retornando 400.");
            return new APIGatewayHttpApiV2ProxyResponse
            {
                StatusCode = 400,
                Body = JsonSerializer.Serialize(new AuthResponse
                {
                    Success = false,
                    Message = "Body da requisição não informado."
                })
            };
        }

        AuthRequest? authRequest;

        try
        {
            authRequest = JsonSerializer.Deserialize<AuthRequest>(request.Body);
            Console.WriteLine("[INFO] Body desserializado com sucesso.");
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"[ERRO] JSON inválido: {ex.Message}");
            return new APIGatewayHttpApiV2ProxyResponse
            {
                StatusCode = 400,
                Body = JsonSerializer.Serialize(new AuthResponse
                {
                    Success = false,
                    Message = "JSON inválido."
                }),
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" }
                }
            };
        }

        if (authRequest == null || string.IsNullOrWhiteSpace(authRequest.Cpf) || !CPFValidator.IsValid(authRequest.Cpf))
        {
            Console.WriteLine("[AVISO] CPF inválido recebido. Retornando 400.");
            return new APIGatewayHttpApiV2ProxyResponse
            {
                StatusCode = 400,
                Body = JsonSerializer.Serialize(new AuthResponse { Success = false, Message = "CPF Inválido" }),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
        }

        var cpf = authRequest.Cpf.Trim();
        Console.WriteLine($"[INFO] CPF validado. Documento normalizado: {cpf}");

        if (_usuarioService == null)
        {
            Console.WriteLine("[ERRO] Serviço de usuário não inicializado. Verifique configuração de banco/secret. Retornando 500.");
            return new APIGatewayHttpApiV2ProxyResponse
            {
                StatusCode = 500,
                Body = JsonSerializer.Serialize(new AuthResponse { Success = false, Message = "Configuração de banco de dados não encontrada." }),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
        }

        UsuarioModel usuario;

        try
        {
            Console.WriteLine($"[INFO] Buscando usuário por documento: {cpf}");
            usuario = _usuarioService.ObterPorDocumentoAsync(cpf).GetAwaiter().GetResult();
            Console.WriteLine($"[INFO] Usuário encontrado: {usuario.Login} | Ativo: {usuario.Ativo}");
        }
        catch (NotFoundException)
        {
            Console.WriteLine("[AVISO] Usuário não encontrado para o documento informado. Retornando 403.");
            return new APIGatewayHttpApiV2ProxyResponse
            {
                StatusCode = 403,
                Body = JsonSerializer.Serialize(new AuthResponse { Success = false, Message = "Cliente inativo ou não encontrado." }),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERRO] Falha ao consultar usuário no banco: {ex.Message}");
            return new APIGatewayHttpApiV2ProxyResponse
            {
                StatusCode = 500,
                Body = JsonSerializer.Serialize(new AuthResponse { Success = false, Message = "Erro interno ao consultar usuário." }),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
        }

        if (!usuario.Ativo)
        {
            Console.WriteLine("[AVISO] Usuário encontrado, porém inativo. Retornando 403.");
            return new APIGatewayHttpApiV2ProxyResponse
            {
                StatusCode = 403,
                Body = JsonSerializer.Serialize(new AuthResponse { Success = false, Message = "Cliente inativo ou não encontrado." }),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
        }

        var jwtToken = _tokenService.GerarToken(usuario);
        Console.WriteLine("[INFO] Token JWT gerado com sucesso.");

        var responseBody = JsonSerializer.Serialize(new { token = jwtToken });
        Console.WriteLine($"[SUCESSO] Gerando resposta: {responseBody}");
        Console.WriteLine($"[FIM] RequestId: {context?.AwsRequestId}");

        return new APIGatewayHttpApiV2ProxyResponse
        {
            StatusCode = 200,
            Body = JsonSerializer.Serialize(new AuthResponse
            {
                Success = true,
                Message = "Autenticado com sucesso.",
                Token = jwtToken
            }),
            Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
        };
    }
}