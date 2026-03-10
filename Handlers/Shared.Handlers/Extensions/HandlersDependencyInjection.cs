using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Shared.Core.Events;

namespace Shared.Handlers.Extensions;

public static class HandlersDependencyInjection
{
    public static IServiceCollection AddHandlersFromAssembly(this IServiceCollection services, Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);

        IEnumerable<Type> types = assembly.GetTypes().Where(type => !type.IsAbstract && !type.IsInterface);

        foreach (Type type in types)
        {
            Type[] typeInterfaces = type.GetInterfaces();

            foreach (Type typeInterface in typeInterfaces)
            {
                if (typeInterface.IsGenericType &&
                    (typeInterface.GetGenericTypeDefinition() == typeof(ICommandHandler<,>) ||
                     typeInterface.GetGenericTypeDefinition() == typeof(ICommandHandler<>) ||
                     typeInterface.GetGenericTypeDefinition() == typeof(IQueryHandler<,>) ||
                     typeInterface.GetGenericTypeDefinition() == typeof(IEventHandler<>)))
                {
                    services.AddTransient(typeInterface, type);
                }
            }
        }

        return services;
    }
}