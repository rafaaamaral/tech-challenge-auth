using TechChallenge.Auth.Utils;

namespace TechChallenge.Auth.Tests.Utils;

public class CPFValidatorTests
{
    [Theory]
    [InlineData("52998224725")]
    [InlineData("529.982.247-25")]
    public void IsValid_DeveRetornarTrue_QuandoCpfForValido(string cpf)
    {
        var resultado = CPFValidator.IsValid(cpf);

        Assert.True(resultado);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("11111111111")]
    [InlineData("12345678900")]
    [InlineData("123")]
    public void IsValid_DeveRetornarFalse_QuandoCpfForInvalido(string cpf)
    {
        var resultado = CPFValidator.IsValid(cpf);

        Assert.False(resultado);
    }
}
