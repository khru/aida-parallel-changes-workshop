namespace Aida.ParallelChange.Api.Infrastructure.Http.Contracts.JsonApi;

public sealed class JsonApiErrorDocument
{
    public List<JsonApiError> Errors { get; init; } = [];
}
