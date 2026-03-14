namespace Aida.ParallelChange.Api.Infrastructure.Http.Contracts.JsonApi;

public sealed class JsonApiError
{
    public string Status { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Detail { get; init; } = string.Empty;
}
