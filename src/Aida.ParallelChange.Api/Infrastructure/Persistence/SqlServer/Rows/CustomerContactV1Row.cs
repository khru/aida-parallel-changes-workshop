namespace Aida.ParallelChange.Api.Infrastructure.Persistence.SqlServer.Rows;

public sealed class CustomerContactV1Row
{
    public int CustomerId { get; init; }
    public string ContactName { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
}
