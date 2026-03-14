using Aida.ParallelChange.Api.Domain;

namespace Aida.ParallelChange.Api.Application.UpdateCustomerContact;

public interface CustomerContactUpdater
{
    Task UpdateAsync(CustomerContact customerContact, CancellationToken cancellationToken = default);
}
