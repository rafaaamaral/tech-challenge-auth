using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using System.Text.Json;
using TechChallenge.Auth.Models;
using TechChallenge.Auth.Utils;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace TechChallenge.Auth;

public class Function
{

    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="input">The event for the Lambda function handler to process.</param>
    /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
    /// <returns></returns>
    public APIGatewayProxyResponse FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
    {
        if (request.HttpMethod != "POST" || !request.Path.EndsWith("/auth/cpf"))
        {
            return new APIGatewayProxyResponse { StatusCode = 404 };
        }

        var authRequest = JsonSerializer.Deserialize<AuthRequest>(request.Body);

        if (authRequest == null || !CPFValidator.IsValid(authRequest.Cpf))
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = 400,
                Body = JsonSerializer.Serialize(new AuthResponse { Success = false, Message = "CPF Inválido" })
            };
        }

        // Lógica futura de geração de JWT (TC3-02) entrará aqui

        return new APIGatewayProxyResponse
        {
            StatusCode = 200,
            Body = JsonSerializer.Serialize(new AuthResponse { Success = true, Message = "CPF validado com sucesso." })
        };
    }
}
