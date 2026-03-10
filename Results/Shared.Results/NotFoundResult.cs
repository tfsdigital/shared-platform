namespace Shared.Results;

public record NotFoundResult : Result
{
    public NotFoundResult(Error error) : base(error)
    {
    }
}

public record NotFoundResult<TValue> : Result<TValue>
{
    public NotFoundResult(Error error) : base(error)
    {
    }
}
