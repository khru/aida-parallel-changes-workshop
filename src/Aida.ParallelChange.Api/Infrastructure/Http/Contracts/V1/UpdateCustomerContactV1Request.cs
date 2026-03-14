namespace Aida.ParallelChange.Api.Infrastructure.Http.Contracts.V1;

public sealed class UpdateCustomerContactV1Request
{
    public string ContactName { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
}
