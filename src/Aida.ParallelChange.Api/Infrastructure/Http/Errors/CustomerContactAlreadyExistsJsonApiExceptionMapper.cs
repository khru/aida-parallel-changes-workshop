using Aida.ParallelChange.Api.Application.CreateCustomerContact;
using Microsoft.AspNetCore.Http;

namespace Aida.ParallelChange.Api.Infrastructure.Http.Errors;

public sealed class CustomerContactAlreadyExistsJsonApiExceptionMapper : IJsonApiExceptionMapper
{
    private const string Title = "Customer contact already exists";

    public bool CanHandle(Exception exception)
    {
        return exception is CustomerContactAlreadyExistsException;
    }

    public JsonApiErrorResponse Map(Exception exception)
    {
        var conflictException = (CustomerContactAlreadyExistsException)exception;
        return new JsonApiErrorResponse(
            StatusCodes.Status409Conflict,
            Title,
            conflictException.Message);
    }
}
