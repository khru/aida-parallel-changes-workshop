namespace Aida.ParallelChange.Api.Application.UpdateCustomerContact;

public sealed class UpdateCustomerContactHandler
{
    private readonly CustomerContactUpdater _updater;

    public UpdateCustomerContactHandler(CustomerContactUpdater updater)
    {
        _updater = updater;
    }

    public Task HandleAsync(UpdateCustomerContactCommand command, CancellationToken cancellationToken = default)
    {
        return _updater.UpdateAsync(command.CustomerContact, cancellationToken);
    }
}
