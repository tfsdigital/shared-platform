# Handlers

CQRS handler interfaces and base classes. Works with MediatR.

## Architecture

Defines `ICommand`, `ICommand<TResponse>`, `IQuery<TResponse>`, `ICommandHandler`, `IQueryHandler`. Base `Handler` and `Handler<TResponse>` provide Result helpers.

## Main Abstractions

```csharp
public interface ICommand;
public interface ICommand<TResponse>;

public interface ICommandHandler<in TCommand> where TCommand : ICommand
{
    Task<Result> Handle(TCommand command, CancellationToken cancellationToken = default);
}

public interface ICommandHandler<in TCommand, TResponse> where TCommand : ICommand<TResponse>
{
    Task<Result<TResponse>> Handle(TCommand command, CancellationToken cancellationToken = default);
}

public interface IQuery<TResponse>;
public interface IQueryHandler<in TQuery, TResponse> where TQuery : IQuery<TResponse>
{
    Task<Result<TResponse>> Handle(TQuery query, CancellationToken cancellationToken = default);
}

public abstract class Handler  // NotFound(), Error(), Success()
public abstract class Handler<TResponse>  // Same for Result<TResponse>
```

## Usage Example

```csharp
public record CreateTabCommand(Guid CashRegisterId, Guid CustomerId) : ICommand<CreateTabCommandResponse>;

public class CreateTabCommandHandler : Handler<CreateTabCommandResponse>
{
    public async Task<Result<CreateTabCommandResponse>> Handle(CreateTabCommand cmd, CancellationToken ct)
    {
        var tab = Tab.Create(...);
        if (tab.IsFailure)
            return Error(tab.Errors[0]);

        await _repository.Add(tab.Data!);
        return Success(new CreateTabCommandResponse(tab.Data!.Id));
    }
}

// Registration
services.AddHandlersFromAssembly(typeof(CreateTabCommandHandler).Assembly);
```
