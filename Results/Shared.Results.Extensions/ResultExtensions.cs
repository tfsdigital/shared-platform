using Microsoft.AspNetCore.Http;

namespace Shared.Results.Extensions;

public static class ResultExtensions
{
    // Extension Methods

    public static IResult Ok<TValue>(this Result<TValue> result) =>
        BaseResult(result, TypedResults.Ok(result.Data));

    public static IResult NoContent(this Result result) =>
        BaseResult(result, TypedResults.NoContent());

    public static IResult Created<TValue>(this Result<TValue> result, string? uri = null) =>
        BaseResult(result, TypedResults.Created(uri, result.Data));

    // Private Methods

    private static IResult BaseResult(Result result, IResult successResult) =>
        result switch
        {
            NotFoundResult => TypedResults.NotFound(result.Errors),
            ErrorResult => TypedResults.BadRequest(result.Errors),
            _ => successResult,
        };

    private static IResult BaseResult<TValue>(Result<TValue> result, IResult successResult) =>
        result switch
        {
            NotFoundResult<TValue> => TypedResults.NotFound(result.Errors),
            ErrorResult<TValue> => TypedResults.BadRequest(result.Errors),
            _ => successResult,
        };
}
