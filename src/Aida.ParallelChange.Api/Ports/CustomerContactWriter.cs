using Aida.ParallelChange.Api.Domain;

namespace Aida.ParallelChange.Api.Ports;

public interface CustomerContactWriter
{
    Task UpsertAsync(CustomerContact customerContact, CancellationToken cancellationToken = default);
}
