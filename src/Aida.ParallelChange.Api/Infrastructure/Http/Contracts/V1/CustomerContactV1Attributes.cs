namespace Aida.ParallelChange.Api.Infrastructure.Http.Contracts.V1;

public sealed class CustomerContactV1Attributes
{
    public string ContactName { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
}
