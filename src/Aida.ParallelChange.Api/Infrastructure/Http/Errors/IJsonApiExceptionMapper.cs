namespace Aida.ParallelChange.Api.Infrastructure.Http.Errors;

public interface IJsonApiExceptionMapper
{
    bool CanHandle(Exception exception);

    JsonApiErrorResponse Map(Exception exception);
}
