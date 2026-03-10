namespace Shared.Results;

public record Result
{
    protected Result()
    {
    }

    protected Result(Error errors) =>
        Errors = [errors];

    protected Result(Error[] errors) =>
        Errors = errors;

    public Error[] Errors { get; } = [];
    public bool IsSuccess => Errors.Length == 0;
    public bool IsFailure => !IsSuccess;

    public static ErrorResult Error(Error error) => new(error);
    public static ErrorResult Error(Error[] errors) => new(errors);
    public static NotFoundResult NotFound(Error error) => new(error);
    public static Result Success() => new();
    public static Result Success<TResponse>(TResponse response) => new Result<TResponse>(response);
}

public record Result<TData> : Result
{
    public TData? Data { get; }

    internal Result(TData data)
    {
        Data = data;
    }

    internal Result(Error notification)
        : base(notification)
    {
    }

    internal Result(Error[] notifications)
        : base(notifications)
    {
    }

    public static new ErrorResult<TData> Error(Error error) => new(error);
    public static new ErrorResult<TData> Error(Error[] errors) => new(errors);
    public static new NotFoundResult<TData> NotFound(Error error) => new(error);
    public static Result<TData> Success(TData data) => new(data);
}