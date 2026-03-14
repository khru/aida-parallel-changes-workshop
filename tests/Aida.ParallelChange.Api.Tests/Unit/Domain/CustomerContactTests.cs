using Aida.ParallelChange.Api.Domain;

namespace Aida.ParallelChange.Api.Tests.Unit.Domain;

[TestFixture]
public sealed class CustomerContactTests
{
    [Test]
    public void Constructor_assigns_value_objects()
    {
        var customerContact = CustomerContactBuilder.FromPrimitives(7, "Grace Hopper", "+1 5550100", "grace.hopper@example.com");

        customerContact.CustomerId.ShouldBe(new CustomerId(7));
        customerContact.ContactName.ShouldBe(new ContactName("Grace Hopper"));
        customerContact.Phone.ShouldBe(new PhoneNumber("+1 5550100"));
        customerContact.Email.ShouldBe(new EmailAddress("grace.hopper@example.com"));
    }
}
