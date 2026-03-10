using FluentValidation;
using Shared.Results;

namespace Shared.Validations;

public class CommandValidator<T>(IValidator<T> validator) : ICommandValidator<T>
{
    public Result Validate(T instance)
    {
        var result = validator.Validate(instance);

        if (result.IsValid)
            return Result.Success();

        var notifications = result.Errors.Select(e =>
            new Error(e.PropertyName, e.ErrorMessage)
        ).ToArray();

        return Result.Error(notifications);
    }
}
