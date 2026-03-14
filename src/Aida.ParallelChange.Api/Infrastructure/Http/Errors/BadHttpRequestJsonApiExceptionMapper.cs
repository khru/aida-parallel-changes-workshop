using Microsoft.AspNetCore.Http;

namespace Aida.ParallelChange.Api.Infrastructure.Http.Errors;

public sealed class BadHttpRequestJsonApiExceptionMapper : IJsonApiExceptionMapper
{
    private const string Title = "Invalid request";
    private const string Detail = "Invalid request.";

    public bool CanHandle(Exception exception)
    {
        return exception is BadHttpRequestException;
    }

    public JsonApiErrorResponse Map(Exception exception)
    {
        return new JsonApiErrorResponse(
            StatusCodes.Status400BadRequest,
            Title,
            Detail);
    }
}
