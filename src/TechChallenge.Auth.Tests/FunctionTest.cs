using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.TestUtilities;
using System.Text.Json;
using TechChallenge.Auth.Models;

namespace TechChallenge.Auth.Tests;

public class FunctionTest
{
    [Fact]
    public void FunctionHandler_DeveRetornar404_QuandoMetodoNaoForPost()
    {
        var function = new Function();
        var request = CriarRequest("GET", "/auth/cpf", "{\"Cpf\":\"52998224725\"}");
        var context = new TestLambdaContext();

        var response = function.FunctionHandler(request, context);

        Assert.Equal(404, response.StatusCode);
    }

    [Fact]
    public void FunctionHandler_DeveRetornar400_QuandoBodyNaoForInformado()
    {
        var function = new Function();
        var request = CriarRequest("POST", "/auth/cpf", null);
        var context = new TestLambdaContext();

        var response = function.FunctionHandler(request, context);

        Assert.Equal(400, response.StatusCode);
        var payload = JsonSerializer.Deserialize<AuthResponse>(response.Body);
        Assert.Equal("Body da requisição não informado.", payload?.Message);
    }

    [Fact]
    public void FunctionHandler_DeveRetornar400_QuandoJsonForInvalido()
    {
        var function = new Function();
        var request = CriarRequest("POST", "/auth/cpf", "{cpf invalido}");
        var context = new TestLambdaContext();

        var response = function.FunctionHandler(request, context);

        Assert.Equal(400, response.StatusCode);
        var payload = JsonSerializer.Deserialize<AuthResponse>(response.Body);
        Assert.Equal("JSON inválido.", payload?.Message);
    }

    [Fact]
    public void FunctionHandler_DeveRetornar400_QuandoCpfForInvalido()
    {
        var function = new Function();
        var request = CriarRequest("POST", "/auth/cpf", "{\"Cpf\":\"123\"}");
        var context = new TestLambdaContext();

        var response = function.FunctionHandler(request, context);

        Assert.Equal(400, response.StatusCode);
        var payload = JsonSerializer.Deserialize<AuthResponse>(response.Body);
        Assert.Equal("CPF Inválido", payload?.Message);
    }

    [Fact]
    public void FunctionHandler_DeveRetornar500_QuandoBancoNaoEstiverConfigurado()
    {
        Environment.SetEnvironmentVariable("DB_SECRET_NAME", null);

        var function = new Function();
        var request = CriarRequest("POST", "/auth/cpf", "{\"Cpf\":\"52998224725\"}");
        var context = new TestLambdaContext();

        var response = function.FunctionHandler(request, context);

        Assert.Equal(500, response.StatusCode);
        var payload = JsonSerializer.Deserialize<AuthResponse>(response.Body);
        Assert.Equal("Configuração de banco de dados não encontrada.", payload?.Message);
    }

    [Fact]
    public void IntegracaoAWS_Function_DeveInicializarComSecret_QuandoAmbienteEstiverConfigurado()
    {
        var executarIntegracao = Environment.GetEnvironmentVariable("RUN_AWS_INTEGRATION_TESTS");
        var secretName = Environment.GetEnvironmentVariable("DB_SECRET_NAME");

        if (!string.Equals(executarIntegracao, "true", StringComparison.OrdinalIgnoreCase) || string.IsNullOrWhiteSpace(secretName))
        {
            return;
        }

        var function = new Function();
        var request = CriarRequest("POST", "/auth/cpf", "{\"Cpf\":\"123\"}");
        var context = new TestLambdaContext();

        var response = function.FunctionHandler(request, context);

        Assert.Equal(400, response.StatusCode);
    }

    private static APIGatewayHttpApiV2ProxyRequest CriarRequest(string method, string path, string? body)
    {
        return new APIGatewayHttpApiV2ProxyRequest
        {
            RequestContext = new APIGatewayHttpApiV2ProxyRequest.ProxyRequestContext
            {
                Http = new APIGatewayHttpApiV2ProxyRequest.HttpDescription
                {
                    Method = method,
                    Path = path
                }
            },
            RawPath = path,
            Body = body
        };
    }
}
