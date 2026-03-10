using FluentValidation;
using FluentValidation.Validators;

namespace Shared.Validations.Extensions;

public class CnpjValidator<T, TElement> : PropertyValidator<T, TElement>
{
    public override string Name => nameof(CnpjValidator<T, TElement>);

    // Protected Methods

    public override bool IsValid(ValidationContext<T> context, TElement value)
    {
        return string.IsNullOrWhiteSpace(value!.ToString()) || ValidateCnpj(value.ToString()!);
    }

    // Private Methods

    private static bool ValidateCnpj(string cnpj)
    {
        cnpj = cnpj.Trim().Replace(".", "").Replace("-", "").Replace("/", "");

        if (cnpj.Length != 14)
        {
            return false;
        }

        var multiplicador1 = new[] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        var multiplicador2 = new[] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

        var tempCnpj = cnpj[..12];

        var soma = 0;
        for (var i = 0; i < 12; i++)
        {
            soma += (tempCnpj[i] - '0') * multiplicador1[i];
        }

        var resto = soma % 11;
        resto = resto < 2 ? 0 : 11 - resto;

        var digito = resto.ToString();

        tempCnpj += digito;
        soma = 0;
        for (var i = 0; i < 13; i++)
        {
            soma += (tempCnpj[i] - '0') * multiplicador2[i];
        }

        resto = soma % 11;
        resto = resto < 2 ? 0 : 11 - resto;

        digito += resto;

        return cnpj.EndsWith(digito);
    }
}
