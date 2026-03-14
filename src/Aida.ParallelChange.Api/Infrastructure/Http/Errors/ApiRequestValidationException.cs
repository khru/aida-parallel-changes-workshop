namespace Aida.ParallelChange.Api.Infrastructure.Http.Errors;

public sealed class ApiRequestValidationException : Exception
{
    public string Title { get; }

    public ApiRequestValidationException(string title, string detail)
        : base(detail)
    {
        Title = title;
    }
}
