using Aida.ParallelChange.Api.Application.UpdateCustomerContact;
using Aida.ParallelChange.Api.Domain;
using NSubstitute;

namespace Aida.ParallelChange.Api.Tests.Unit.Application;

[TestFixture]
public sealed class UpdateCustomerContactHandlerTests
{
    [Test]
    public async Task HandleAsync_calls_port_to_update_contact()
    {
        var updater = Substitute.For<CustomerContactUpdater>();
        var contact = CustomerContactBuilder.FromPrimitives(7, "Grace Hopper", "+1 5550100", "grace.hopper@example.com");
        var command = new UpdateCustomerContactCommand(contact);
        var handler = new UpdateCustomerContactHandler(updater);

        await handler.HandleAsync(command, CancellationToken.None);

        await updater.Received(1).UpdateAsync(contact, Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task HandleAsync_propagates_not_found_error()
    {
        var updater = Substitute.For<CustomerContactUpdater>();
        var contact = CustomerContactBuilder.FromPrimitives(7, "Grace Hopper", "+1 5550100", "grace.hopper@example.com");
        var command = new UpdateCustomerContactCommand(contact);
        var handler = new UpdateCustomerContactHandler(updater);

        updater
            .UpdateAsync(contact, Arg.Any<CancellationToken>())
            .Returns(_ => throw new CustomerContactNotFoundException(7));

        var exception = await Should.ThrowAsync<CustomerContactNotFoundException>(() => handler.HandleAsync(command, CancellationToken.None));

        exception.CustomerId.ShouldBe(7);
    }
}
