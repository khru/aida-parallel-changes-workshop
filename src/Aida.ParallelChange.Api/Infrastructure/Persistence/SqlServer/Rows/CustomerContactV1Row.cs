namespace Aida.ParallelChange.Api.Infrastructure.Persistence.SqlServer.Rows;

public sealed class CustomerContactV1Row
{
    public int CustomerId { get; init; }
    public string? ContactName { get; init; }
    public string? Phone { get; init; }
    public string Email { get; init; } = string.Empty;
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? PhoneCountryCode { get; init; }
    public string? PhoneLocalNumber { get; init; }
}
