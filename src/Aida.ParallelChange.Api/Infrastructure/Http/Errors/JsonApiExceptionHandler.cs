using Aida.ParallelChange.Api.Infrastructure.Http.Contracts.JsonApi;
using Aida.ParallelChange.Api.Infrastructure.Http.JsonApi;
using Microsoft.AspNetCore.Diagnostics;

namespace Aida.ParallelChange.Api.Infrastructure.Http.Errors;

public sealed class JsonApiExceptionHandler : IExceptionHandler
{
    private readonly JsonApiExceptionMapperFactory _mapperFactory;

    public JsonApiExceptionHandler(JsonApiExceptionMapperFactory mapperFactory)
    {
        _mapperFactory = mapperFactory;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var response = _mapperFactory.Create(exception);

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
}
