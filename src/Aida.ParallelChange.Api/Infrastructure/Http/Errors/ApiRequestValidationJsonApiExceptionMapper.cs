using Microsoft.AspNetCore.Http;

namespace Aida.ParallelChange.Api.Infrastructure.Http.Errors;

public sealed class ApiRequestValidationJsonApiExceptionMapper : IJsonApiExceptionMapper
{
    public bool CanHandle(Exception exception)
    {
        return exception is ApiRequestValidationException;
    }

    public JsonApiErrorResponse Map(Exception exception)
    {
        var validationException = (ApiRequestValidationException)exception;
        return new JsonApiErrorResponse(
            StatusCodes.Status400BadRequest,
            validationException.Title,
            validationException.Message);
    }
}
