using Microsoft.AspNetCore.Http;

namespace Aida.ParallelChange.Api.Infrastructure.Http.Errors;

public static class JsonApiErrorCatalog
{
    public const string InvalidCustomerIdTitle = "Invalid customer id";
    public const string InvalidRequestTitle = "Invalid request";
    public const string InvalidRequestDetail = "Invalid request.";
    public const string CustomerContactNotFoundTitle = "Customer contact not found";
    public const string CustomerContactAlreadyExistsTitle = "Customer contact already exists";
    public const string UnexpectedErrorTitle = "Unexpected error";
    public const string InvalidCustomerIdNumberDetail = "Customer id must be a number greater than zero.";
    public const string UnexpectedErrorDetail = "An unexpected error occurred while processing the request.";

    public static readonly int InvalidRequestStatusCode = StatusCodes.Status400BadRequest;
    public static readonly int CustomerContactNotFoundStatusCode = StatusCodes.Status404NotFound;
    public static readonly int CustomerContactAlreadyExistsStatusCode = StatusCodes.Status409Conflict;
    public static readonly int UnexpectedErrorStatusCode = StatusCodes.Status500InternalServerError;
}
