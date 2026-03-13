using Aida.ParallelChange.Api.Ports;

namespace Aida.ParallelChange.Api.Application.UpdateCustomerContact;

public sealed class UpdateCustomerContactHandler
{
    private readonly CustomerContactWriter _writer;

    public UpdateCustomerContactHandler(CustomerContactWriter writer)
    {
        _writer = writer;
    }

    public Task HandleAsync(UpdateCustomerContactCommand command, CancellationToken cancellationToken = default)
    {
        return _writer.UpsertAsync(command.CustomerContact, cancellationToken);
    }
}
