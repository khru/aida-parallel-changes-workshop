namespace Aida.ParallelChange.Api.Application.CreateCustomerContact;

public sealed class CreateCustomerContactHandler
{
    private readonly CustomerContactCreator _creator;

    public CreateCustomerContactHandler(CustomerContactCreator creator)
    {
        _creator = creator;
    }

    public Task HandleAsync(CreateCustomerContactCommand command, CancellationToken cancellationToken = default)
    {
        return _creator.CreateAsync(command.CustomerContact, cancellationToken);
    }
}
