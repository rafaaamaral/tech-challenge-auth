using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.TestUtilities;

namespace TechChallenge.Auth.Tests;

public class FunctionTest
{
    [Fact]
    public void TestFunctionHandler_InvalidCpf_Returns400()
    {
        var function = new Function();
        var request = new APIGatewayProxyRequest
        {
            HttpMethod = "POST",
            Path = "/auth/cpf",
            Body = "{\"Cpf\":\"123\"}"
        };
        var context = new TestLambdaContext();

        var response = function.FunctionHandler(request, context);

        Assert.Equal(400, response.StatusCode);
    }
}
