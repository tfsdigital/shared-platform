using FluentValidation;

namespace Shared.Validations.Extensions;

public static class ValidatorExtensions
{
    public static IRuleBuilderOptions<T, TElement> Cnpj<T, TElement>(this IRuleBuilder<T, TElement> ruleBuilder)
    {
        return ruleBuilder.SetValidator(new CnpjValidator<T, TElement>());
    }

    public static IRuleBuilderOptions<T, TElement> Cpf<T, TElement>(this IRuleBuilder<T, TElement> ruleBuilder)
    {
        return ruleBuilder.SetValidator(new CpfValidator<T, TElement>());
    }
}
