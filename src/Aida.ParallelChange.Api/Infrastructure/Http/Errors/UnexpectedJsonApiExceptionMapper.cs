using Microsoft.AspNetCore.Http;

namespace Aida.ParallelChange.Api.Infrastructure.Http.Errors;

public sealed class UnexpectedJsonApiExceptionMapper : IJsonApiExceptionMapper
{
    private const string Title = "Unexpected error";
    private const string Detail = "An unexpected error occurred while processing the request.";

    public bool CanHandle(Exception exception)
    {
        return true;
    }

    public JsonApiErrorResponse Map(Exception exception)
    {
        return new JsonApiErrorResponse(
            StatusCodes.Status500InternalServerError,
            Title,
            Detail);
    }
}
