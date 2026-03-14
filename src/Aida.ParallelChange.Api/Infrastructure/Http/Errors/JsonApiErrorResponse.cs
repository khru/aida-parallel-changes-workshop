namespace Aida.ParallelChange.Api.Infrastructure.Http.Errors;

public sealed record JsonApiErrorResponse(int StatusCode, string Title, string Detail);
