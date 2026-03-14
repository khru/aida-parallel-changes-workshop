namespace Aida.ParallelChange.Api.Infrastructure.Http.Contracts.JsonApi;

public sealed class JsonApiResource<TAttributes>
{
    public string Type { get; init; } = string.Empty;
    public string Id { get; init; } = string.Empty;
    public TAttributes Attributes { get; init; } = default!;
}
