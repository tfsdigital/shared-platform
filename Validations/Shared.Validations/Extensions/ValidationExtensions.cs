using Microsoft.Extensions.DependencyInjection;

namespace Shared.Validations.Extensions;

public static class ValidationExtensions
{
    public static void AddValidations(this IServiceCollection services)
    {
        services.AddTransient(typeof(ICommandValidator<>), typeof(CommandValidator<>));
    }
}
