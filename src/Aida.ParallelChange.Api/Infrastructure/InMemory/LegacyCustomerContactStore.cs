using System.Collections.Concurrent;

namespace Aida.ParallelChange.Api.Infrastructure.InMemory;

public sealed class LegacyCustomerContactStore
{
    private readonly ConcurrentDictionary<int, LegacyCustomerContactRecord> _records = new();

    public LegacyCustomerContactStore()
    {
        _records[42] = new LegacyCustomerContactRecord(
            42,
            "María García",
            "+34 600123123",
            "maria.garcia@example.com");
    }

    public LegacyCustomerContactRecord? FindById(int customerId)
    {
        _records.TryGetValue(customerId, out var record);
        return record;
    }
}

public sealed record LegacyCustomerContactRecord(
    int CustomerId,
    string ContactName,
    string Phone,
    string Email);
