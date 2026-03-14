using Aida.ParallelChange.Api.Application.GetCustomerContact;
using Aida.ParallelChange.Api.Domain;
using NSubstitute;

namespace Aida.ParallelChange.Api.Tests.Unit.Application;

[TestFixture]
public sealed class GetCustomerContactHandlerTests
{
    [Test]
    public async Task HandleAsync_returns_contact_from_reader()
    {
        var reader = Substitute.For<CustomerContactReader>();
        var query = new GetCustomerContactQuery(new CustomerId(7));
        var contact = CustomerContactBuilder.FromPrimitives(7, "Grace Hopper", "+1 5550100", "grace.hopper@example.com");

        reader
            .FindByIdAsync(query.CustomerId, Arg.Any<CancellationToken>())
            .Returns(new FindCustomerContactResult.Found(contact));

        var handler = new GetCustomerContactHandler(reader);
        var result = await handler.HandleAsync(query, CancellationToken.None);
        var found = result as FindCustomerContactResult.Found;

        found.ShouldNotBeNull();
        found.CustomerContact.CustomerId.Value.ShouldBe(7);
    }

    [Test]
    public async Task HandleAsync_returns_not_found_when_reader_finds_nothing()
    {
        var reader = Substitute.For<CustomerContactReader>();
        var query = new GetCustomerContactQuery(new CustomerId(8));

        reader
            .FindByIdAsync(query.CustomerId, Arg.Any<CancellationToken>())
            .Returns(new FindCustomerContactResult.NotFound(query.CustomerId));

        var handler = new GetCustomerContactHandler(reader);
        var result = await handler.HandleAsync(query, CancellationToken.None);
        var notFound = result as FindCustomerContactResult.NotFound;

        notFound.ShouldNotBeNull();
        notFound.CustomerId.Value.ShouldBe(8);
    }
}
