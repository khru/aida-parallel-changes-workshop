using Aida.ParallelChange.Api.Domain;

namespace Aida.ParallelChange.Api.Application.GetCustomerContact;

public interface CustomerContactReader
{
    Task<FindCustomerContactResult> FindByIdAsync(CustomerId customerId, CancellationToken cancellationToken = default);
}
