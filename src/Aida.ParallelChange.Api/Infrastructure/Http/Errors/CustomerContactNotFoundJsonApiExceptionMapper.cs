using Aida.ParallelChange.Api.Application.UpdateCustomerContact;
using Microsoft.AspNetCore.Http;

namespace Aida.ParallelChange.Api.Infrastructure.Http.Errors;

public sealed class CustomerContactNotFoundJsonApiExceptionMapper : IJsonApiExceptionMapper
{
    private const string Title = "Customer contact not found";

    public bool CanHandle(Exception exception)
    {
        return exception is CustomerContactNotFoundException;
    }

    public JsonApiErrorResponse Map(Exception exception)
    {
        var notFoundException = (CustomerContactNotFoundException)exception;
        return new JsonApiErrorResponse(
            StatusCodes.Status404NotFound,
            Title,
            notFoundException.Message);
    }
}
