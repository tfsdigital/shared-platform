using Shared.Results;

namespace Shared.Handlers;

public abstract class Handler<TResponse>
{
    protected static Result<TResponse> NotFound(Error notification) =>
        Result<TResponse>.NotFound(notification);

    protected static Result<TResponse> Error(Error notification) =>
        Result<TResponse>.Error(notification);

    protected static Result<TResponse> Error(Error[] notifications) =>
        Result<TResponse>.Error(notifications);

    protected static Result<TResponse> Success(TResponse response) =>
        Result<TResponse>.Success(response);
}

public abstract class Handler
{
    protected static Result NotFound(Error notification) =>
        Result.NotFound(notification);

    protected static Result Error(Error notification) =>
        Result.Error(notification);

    protected static Result Error(Error[] notifications) =>
        Result.Error(notifications);

    protected static Result Success() =>
        Result.Success();

    protected static Result Success<TResponse>(TResponse response) =>
        Result.Success(response);
}