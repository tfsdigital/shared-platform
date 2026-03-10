using Shared.Results;

namespace Shared.Validations;

public interface ICommandValidator<in T>
{
    Result Validate(T instance);
}
