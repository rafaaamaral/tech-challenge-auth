using System;
using System.Collections.Generic;
using System.Text;

namespace TechChallenge.Auth.Utils
{
    public static class CPFValidator
    {
        public static bool IsValid(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf)) return false;

            var cleanCpf = new string(cpf.Where(char.IsDigit).ToArray());
            if (cleanCpf.Length != 11 || cleanCpf.Distinct().Count() == 1)
                return false;

            var soma = 0;
            for (int i = 0; i < 9; i++)
                soma += (cleanCpf[i] - '0') * (10 - i);

            var resto = soma % 11;
            var dig1 = resto < 2 ? 0 : 11 - resto;

            if (cleanCpf[9] - '0' != dig1)
                return false;

            soma = 0;
            for (int i = 0; i < 10; i++)
                soma += (cleanCpf[i] - '0') * (11 - i);

            resto = soma % 11;
            var dig2 = resto < 2 ? 0 : 11 - resto;

            return cleanCpf[10] - '0' == dig2;
        }
    }
}
