using System.Collections.Concurrent;
using Aida.ParallelChange.Api.Application.CreateCustomerContact;
using Aida.ParallelChange.Api.Application.GetCustomerContact;
using Aida.ParallelChange.Api.Application.UpdateCustomerContact;
using Aida.ParallelChange.Api.Domain;

namespace Aida.ParallelChange.Api.Infrastructure.InMemory;

public sealed class InMemoryCustomerContactRepository : CustomerContactReader, CustomerContactCreator, CustomerContactUpdater
{
    private readonly ConcurrentDictionary<int, CustomerContact> _records = new();

    public Task<FindCustomerContactResult> FindByIdAsync(CustomerId customerId, CancellationToken cancellationToken = default)
    {
        if (_records.TryGetValue(customerId.Value, out var contact))
        {
            return Task.FromResult<FindCustomerContactResult>(new FindCustomerContactResult.Found(contact));
        }

        return Task.FromResult<FindCustomerContactResult>(new FindCustomerContactResult.NotFound(customerId));
    }

    public Task CreateAsync(CustomerContact customerContact, CancellationToken cancellationToken = default)
    {
        if (!_records.TryAdd(customerContact.CustomerId.Value, customerContact))
        {
            throw new CustomerContactAlreadyExistsException(customerContact.CustomerId.Value);
        }

        return Task.CompletedTask;
    }

    public Task UpdateAsync(CustomerContact customerContact, CancellationToken cancellationToken = default)
    {
        if (!_records.ContainsKey(customerContact.CustomerId.Value))
        {
            throw new CustomerContactNotFoundException(customerContact.CustomerId.Value);
        }

        _records[customerContact.CustomerId.Value] = customerContact;
        return Task.CompletedTask;
    }
}
