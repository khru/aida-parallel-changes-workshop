using Aida.ParallelChange.Api.Domain;

namespace Aida.ParallelChange.Api.Tests.Unit.Domain;

[TestFixture]
public sealed class CustomerContactTests
{
    [Test]
    public void Constructor_assigns_value_objects()
    {
        var customerContact = CustomerContactBuilder.FromPrimitives(7, "Grace Hopper", "+1 5550100", "grace.hopper@example.com");

        customerContact.CustomerId.Value.ShouldBe(7);
        customerContact.ContactName.Value.ShouldBe("Grace Hopper");
        customerContact.Phone.Value.ShouldBe("+1 5550100");
        customerContact.Email.Value.ShouldBe("grace.hopper@example.com");
    }
}
