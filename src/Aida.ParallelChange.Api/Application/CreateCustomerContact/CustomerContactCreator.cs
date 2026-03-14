using Aida.ParallelChange.Api.Domain;

namespace Aida.ParallelChange.Api.Application.CreateCustomerContact;

public interface CustomerContactCreator
{
    Task CreateAsync(CustomerContact customerContact, CancellationToken cancellationToken = default);
}
