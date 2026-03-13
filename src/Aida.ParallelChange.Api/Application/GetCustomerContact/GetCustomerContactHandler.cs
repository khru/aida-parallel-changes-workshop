using Aida.ParallelChange.Api.Domain;
using Aida.ParallelChange.Api.Ports;

namespace Aida.ParallelChange.Api.Application.GetCustomerContact;

public sealed class GetCustomerContactHandler
{
    private readonly CustomerContactReader _reader;

    public GetCustomerContactHandler(CustomerContactReader reader)
    {
        _reader = reader;
    }

    public Task<CustomerContact?> HandleAsync(GetCustomerContactQuery query, CancellationToken cancellationToken = default)
    {
        return _reader.FindByIdAsync(query.CustomerId, cancellationToken);
    }
}
