using FluentValidation;
using FluentValidation.Validators;

namespace Shared.Validations.Extensions;

public class CpfValidator<T, TElement> : PropertyValidator<T, TElement>
{
    public override string Name => nameof(CpfValidator<T, TElement>);

    // Protected Methods

    public override bool IsValid(ValidationContext<T> context, TElement value)
    {
        return string.IsNullOrWhiteSpace(value!.ToString()) || ValidateCpf(value.ToString()!);
    }

    // Private Methods

    private static bool ValidateCpf(string cpf)
    {
        var multiplicador1 = new[] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        var multiplicador2 = new[] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

        cpf = cpf.Trim();
        cpf = cpf.Replace(".", "").Replace("-", "");

        if (cpf.Length != 11)
        {
            return false;
        }

        var tempCpf = cpf[..9];
        var soma = 0;

        for (var i = 0; i < 9; i++)
        {
            soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];
        }

        var resto = soma % 11;

        resto = resto < 2 ? 0 : 11 - resto;

        var digito = resto.ToString();
        tempCpf += digito;
        soma = 0;

        for (var i = 0; i < 10; i++)
        {
            soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];
        }

        resto = soma % 11;

        resto = resto < 2 ? 0 : 11 - resto;

        digito += resto;

        return cpf.EndsWith(digito);
    }

}