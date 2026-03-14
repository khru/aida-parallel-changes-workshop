namespace Aida.ParallelChange.Api.Infrastructure.Http.Contracts.JsonApi;

public class JsonApiDataDocument<TAttributes>
{
    public JsonApiResource<TAttributes> Data { get; init; } = default!;
}
