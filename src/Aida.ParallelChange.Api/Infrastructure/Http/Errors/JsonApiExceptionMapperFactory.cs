namespace Aida.ParallelChange.Api.Infrastructure.Http.Errors;

public sealed class JsonApiExceptionMapperFactory
{
    private readonly IReadOnlyList<IJsonApiExceptionMapper> _mappers;

    public JsonApiExceptionMapperFactory(IEnumerable<IJsonApiExceptionMapper> mappers)
    {
        _mappers = mappers.ToList();
    }

    public JsonApiErrorResponse Create(Exception exception)
    {
        foreach (var mapper in _mappers)
        {
            if (mapper.CanHandle(exception))
            {
                return mapper.Map(exception);
            }
        }

        throw new InvalidOperationException("No JSON:API exception mapper is configured.");
    }
}
