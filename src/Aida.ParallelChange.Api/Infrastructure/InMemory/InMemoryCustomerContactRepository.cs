using System.Collections.Concurrent;
using Aida.ParallelChange.Api.Domain;
using Aida.ParallelChange.Api.Ports;

namespace Aida.ParallelChange.Api.Infrastructure.InMemory;

public sealed class InMemoryCustomerContactRepository : CustomerContactReader, CustomerContactWriter
{
    private readonly ConcurrentDictionary<int, CustomerContact> _records = new();

    public InMemoryCustomerContactRepository()
    {
        _records[42] = new CustomerContact(
            new CustomerId(42),
            "María García",
            "+34 600123123",
            new EmailAddress("maria.garcia@example.com"));
    }

    public Task<CustomerContact?> FindByIdAsync(CustomerId customerId, CancellationToken cancellationToken = default)
    {
        _records.TryGetValue(customerId.Value, out var contact);
        return Task.FromResult(contact);
    }

    public Task UpsertAsync(CustomerContact customerContact, CancellationToken cancellationToken = default)
    {
        _records[customerContact.CustomerId.Value] = customerContact;
        return Task.CompletedTask;
    }
}
