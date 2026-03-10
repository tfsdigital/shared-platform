namespace Shared.Results;

public record ErrorResult : Result
{
    public ErrorResult(Error error) : base(error)
    {
    }

    public ErrorResult(Error[] errors) : base(errors)
    {
    }
}

public record ErrorResult<TValue> : Result<TValue>
{
    public ErrorResult(Error error) : base(error)
    {
    }

    public ErrorResult(Error[] errors) : base(errors)
    {
    }
}