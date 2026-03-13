using Aida.ParallelChange.Api.Domain;

namespace Aida.ParallelChange.Api.Ports;

public interface CustomerContactReader
{
    Task<CustomerContact?> FindByIdAsync(CustomerId customerId, CancellationToken cancellationToken = default);
}
