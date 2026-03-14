using Aida.ParallelChange.Api.Application.CreateCustomerContact;
using Aida.ParallelChange.Api.Domain;
using NSubstitute;

namespace Aida.ParallelChange.Api.Tests.Unit.Application;

[TestFixture]
public sealed class CreateCustomerContactHandlerTests
{
    [Test]
    public async Task HandleAsync_calls_port_to_create_contact()
    {
        var creator = Substitute.For<CustomerContactCreator>();
        var contact = CustomerContactBuilder.FromPrimitives(7, "Grace Hopper", "+1 5550100", "grace.hopper@example.com");
        var command = new CreateCustomerContactCommand(contact);
        var handler = new CreateCustomerContactHandler(creator);

        await handler.HandleAsync(command, CancellationToken.None);

        await creator.Received(1).CreateAsync(contact, Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task HandleAsync_propagates_conflict_error()
    {
        var creator = Substitute.For<CustomerContactCreator>();
        var contact = CustomerContactBuilder.FromPrimitives(7, "Grace Hopper", "+1 5550100", "grace.hopper@example.com");
        var command = new CreateCustomerContactCommand(contact);
        var handler = new CreateCustomerContactHandler(creator);

        creator
            .CreateAsync(contact, Arg.Any<CancellationToken>())
            .Returns(_ => throw new CustomerContactAlreadyExistsException(7));

        var exception = await Should.ThrowAsync<CustomerContactAlreadyExistsException>(() => handler.HandleAsync(command, CancellationToken.None));

        exception.CustomerId.ShouldBe(7);
    }
}
