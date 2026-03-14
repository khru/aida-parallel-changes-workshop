using Aida.ParallelChange.Api.Application.CreateCustomerContact;
using Aida.ParallelChange.Api.Application.UpdateCustomerContact;
using Aida.ParallelChange.Api.Infrastructure.Http.Contracts.JsonApi;
using Aida.ParallelChange.Api.Infrastructure.Http.JsonApi;
using Microsoft.AspNetCore.Diagnostics;

namespace Aida.ParallelChange.Api.Infrastructure.Http.Errors;

public sealed class JsonApiExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var response = MapException(exception);

        httpContext.Response.StatusCode = response.StatusCode;

        var document = new JsonApiErrorDocument
        {
            Errors =
            [
                new JsonApiError
                {
                    Status = response.StatusCode.ToString(),
                    Title = response.Title,
                    Detail = response.Detail
                }
            ]
        };

        await httpContext.Response.WriteAsJsonAsync(
            document,
            options: null,
            contentType: JsonApiMediaTypes.JsonApi,
            cancellationToken: cancellationToken);
        return true;
    }

    private static JsonApiErrorResponse MapException(Exception exception)
    {
        if (exception is ApiRequestValidationException validationException)
        {
            return new JsonApiErrorResponse(
                JsonApiErrorCatalog.InvalidRequestStatusCode,
                validationException.Title,
                validationException.Message);
        }

        if (exception is CustomerContactAlreadyExistsException alreadyExistsException)
        {
            return new JsonApiErrorResponse(
                JsonApiErrorCatalog.CustomerContactAlreadyExistsStatusCode,
                JsonApiErrorCatalog.CustomerContactAlreadyExistsTitle,
                alreadyExistsException.Message);
        }

        if (exception is CustomerContactNotFoundException notFoundException)
        {
            return new JsonApiErrorResponse(
                JsonApiErrorCatalog.CustomerContactNotFoundStatusCode,
                JsonApiErrorCatalog.CustomerContactNotFoundTitle,
                notFoundException.Message);
        }

        if (exception is BadHttpRequestException)
        {
            return new JsonApiErrorResponse(
                JsonApiErrorCatalog.InvalidRequestStatusCode,
                JsonApiErrorCatalog.InvalidRequestTitle,
                JsonApiErrorCatalog.InvalidRequestDetail);
        }

        return new JsonApiErrorResponse(
            JsonApiErrorCatalog.UnexpectedErrorStatusCode,
            JsonApiErrorCatalog.UnexpectedErrorTitle,
            JsonApiErrorCatalog.UnexpectedErrorDetail);
    }

    private sealed record JsonApiErrorResponse(int StatusCode, string Title, string Detail);
}
